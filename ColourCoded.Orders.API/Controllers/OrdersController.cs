using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ColourCoded.Orders.API.Data;
using ColourCoded.Orders.API.Models.RequestModels.Orders;
using ColourCoded.Orders.API.Models.ResponseModels;
using System;
using ColourCoded.Orders.API.Data.Entities.Orders;
using ColourCoded.Orders.API.Shared;
using Microsoft.EntityFrameworkCore;

namespace ColourCoded.Orders.API.Controllers
{
  [Route("api/orders")]
  public class OrdersController : Controller
  {
    protected OrdersContext Context;

    public OrdersController(OrdersContext context)
    {
      Context = context;
    }

    [HttpPost, Route("vatrate")]
    public decimal GetVatRate()
    {
      var vatTax = Context.TaxRates.First(r => r.TaxCode == OrdersConstants.VatTaxCode && r.StartDate.Date <= DateTime.Now.Date && r.EndDate.Date >= DateTime.Now.Date);

      return vatTax.Rate;
    }

    [HttpPost, Route("getseed")]
    public int GetOrderNoSeed([FromBody]GetCompanyOrderNoSeedRequestModel requestModel)
    {
      if(requestModel.CompanyProfileId != 0)
        return Context.CompanyProfiles.First(cp => cp.CompanyProfileId == requestModel.CompanyProfileId).OrderNoSeed;

      return 1; // default seed value
    }

    [HttpPost, Route("home")]
    public List<HomeOrdersModel> GetHomeOrders([FromBody]GetHomeOrdersRequestModel requestModel)
    {
      var orders = new List<HomeOrdersModel>();

      var userOrders = Context.Orders.Where(o => o.CreateUser == requestModel.Username).OrderByDescending(o => o.CreateDate)
        .Take(50).Select(o => new HomeOrdersModel
        {
          OrderId = o.OrderId,
          OrderNo = o.OrderNo,
          CreateDate = o.CreateDate.ToShortDateString(),
          Total = o.OrderTotal.ToString("R # ###.#0")
        }).ToList();

      if (userOrders != null) orders.AddRange(userOrders);

      if (requestModel.CompanyProfileId != 0)
      {
        var companyOrders = Context.Orders.Where(o => o.CreateUser != requestModel.Username && o.CompanyProfileId == requestModel.CompanyProfileId).OrderByDescending(o => o.CreateDate)
          .Take(50).Select(o => new HomeOrdersModel
          {
            OrderId = o.OrderId,
            OrderNo = o.OrderNo,
            CreateDate = o.CreateDate.ToShortDateString(),
            Total = o.OrderTotal.ToString("R # ###.#0")
          }).ToList();

        if (companyOrders != null) orders.AddRange(companyOrders);
      }

      return orders;
    }

    [HttpPost, Route("homeperiod")]
    public List<HomeOrdersModel> GetHomeOrdersInPeriod([FromBody]GetHomeOrdersPeriodRequestModel requestModel)
    {
      var orders = new List<HomeOrdersModel>();

      var userOrders = Context.Orders.Where(o => o.CreateUser == requestModel.Username && o.CreateDate.Date >= requestModel.StartDate && o.CreateDate.Date <= requestModel.EndDate)
        .Select(o => new HomeOrdersModel
        {
          OrderId = o.OrderId,
          OrderNo = o.OrderNo,
          CreateDate = o.CreateDate.ToShortDateString(),
          Total = o.OrderTotal.ToString("R # ###.#0")
        }).ToList();

      if (userOrders != null) orders.AddRange(userOrders);

      if (requestModel.CompanyProfileId != 0)
      {
        var companyOrders = Context.Orders.Where(o => o.CreateUser != requestModel.Username && o.CompanyProfileId == requestModel.CompanyProfileId && o.CreateDate.Date >= requestModel.StartDate && o.CreateDate.Date <= requestModel.EndDate).OrderByDescending(o => o.CreateDate)
          .Take(50).Select(o => new HomeOrdersModel
          {
            OrderId = o.OrderId,
            OrderNo = o.OrderNo,
            CreateDate = o.CreateDate.ToShortDateString(),
            Total = o.OrderTotal.ToString("R # ###.#0")
          }).ToList();

        if (companyOrders != null) orders.AddRange(companyOrders);
      }

      return orders;
    }

    [HttpPost, Route("add")]
    public int AddOrder([FromBody]AddOrderRequestModel requestModel)
    {
      var existingOrder = Context.Orders.FirstOrDefault(o => o.OrderNo == requestModel.OrderNo);
      var vatTax = Context.TaxRates.First(r => r.TaxCode == OrdersConstants.VatTaxCode && r.StartDate.Date <= DateTime.Now.Date && r.EndDate.Date >= DateTime.Now.Date);

      if (existingOrder != null)
        return existingOrder.OrderId;

      var newOrder = new OrderHead
      {
        OrderNo = requestModel.OrderNo,
        SubTotal = 0M,
        VatTotal = 0M,
        DiscountTotal = 0M,
        OrderTotal = 0M,
        VatRate = vatTax.Rate,
        CompanyProfileId = requestModel.CompanyProfileId,
        CreateDate = DateTime.Now,
        CreateUser = requestModel.Username
      };

      Context.Orders.Add(newOrder);

      if(requestModel.CompanyProfileId != 0)
        Context.CompanyProfiles.First(cp => cp.CompanyProfileId == requestModel.CompanyProfileId).OrderNoSeed += 1;

      Context.SaveChanges();

      return newOrder.OrderId; 
    }

    [HttpPost, Route("orderdetail/add")]
    public ValidationResult AddOrderDetail([FromBody]List<AddOrderDetailRequestModel> requestModel)
    {
      var validationResult = new ValidationResult();
      var subTotal = 0M;
      var vatTotal = 0M;
      var discount = 0M;
      var orderTotal = 0M;

      // check if the line totals are calculated correctly else invalidate
      if (requestModel.Any(o => o.LineTotal != (o.Quantity * o.UnitPrice - o.Discount)))
      {
        validationResult.InValidate("", "Incorrect line total");
        return validationResult;
      }

      // check if there are any values less than zero else invalidate
      if (requestModel.Any(o => o.Quantity < 0 || o.UnitPrice < 0 || o.Discount < 0 || o.LineTotal < 0))
      {
        validationResult.InValidate("", "No values may be less than zero");
        return validationResult;
      }

      // happy path
      if (requestModel.Count > 0)
      {
        var orderId = requestModel[0].OrderId;
        var lineNo = requestModel[0].LineNo;

        // check if all the order id's match
        if (requestModel.Any(o => o.OrderId != orderId))
        {
          validationResult.InValidate("", "Not all Order Id's match.");
          return validationResult;
        }

        // check if all the line no's match
        if (requestModel.Any(o => o.LineNo != lineNo))
        {
          validationResult.InValidate("", "Not all Line No's match.");
          return validationResult;
        }

        var order = Context.Orders.Include(o => o.OrderDetails).First(o => o.OrderId == orderId);

        // add new order details
        // calculate all the head info: subtotal, discount, vattotal, ordertotal
        foreach (var lineDetail in requestModel)
        {
          var orderDetail = new OrderDetail
          {
            LineNo = lineDetail.LineNo,
            OrderId = lineDetail.OrderId,
            ItemDescription = lineDetail.ItemDescription,
            UnitPrice = lineDetail.UnitPrice,
            Quantity = lineDetail.Quantity,
            Discount = lineDetail.Discount,
            LineTotal = lineDetail.LineTotal,
            CreateDate = DateTime.Now,
            CreateUser = lineDetail.Username
          };

          order.OrderDetails.Add(orderDetail);

          var lineDetailVat = lineDetail.LineTotal * order.VatRate;

          subTotal += lineDetail.LineTotal;
          discount += lineDetail.Discount;
          vatTotal += lineDetailVat;
          orderTotal += lineDetail.LineTotal + lineDetailVat;
        }

        // negate all other order details
        foreach(var lineDetail in order.OrderDetails.Where(o => o.LineNo != lineNo))
        {
          lineDetail.Negate = true;
          lineDetail.UpdateDate = DateTime.Now;
          lineDetail.UpdateUser = requestModel[0].Username;
        }

        // update the orderhead
        order.SubTotal = subTotal;
        order.DiscountTotal = discount;
        order.VatTotal = vatTotal;
        order.OrderTotal = orderTotal;

        Context.SaveChanges();
      }

      return validationResult;
    }

    [HttpPost, Route("lineno")]
    public int GetOrderLineNo([FromBody]GetOrderDetailLineNoRequestModel requestModel)
    {
      var existingOrder = Context.Orders.Include(o => o.OrderDetails).First(o => o.OrderId == requestModel.OrderId);

      if (existingOrder.OrderDetails.Count == 0)
        return 1;

      return (existingOrder.OrderDetails.Max(o => o.LineNo) + 1);
    }

    [HttpPost, Route("orderdetail/get")]
    public OrderDetailModel GetOrderDetail([FromBody]GetOrderDetailRequestModel requestModel)
    {
      var existingOrder = Context.Orders.Include(o => o.OrderDetails).Single(o => o.OrderId == requestModel.OrderId);

      var result = new OrderDetailModel
      {
        OrderId = existingOrder.OrderId,
        OrderNo = existingOrder.OrderNo,
        CreateDate = existingOrder.CreateDate,
        SubTotal = existingOrder.SubTotal,
        VatTotal = existingOrder.VatTotal,
        Total = existingOrder.OrderTotal,
        Discount = existingOrder.DiscountTotal,
        OrderLineDetails = new List<OrderLineDetailModel>()
      };

      var maxLineNo = existingOrder.OrderDetails.Max(o => o.LineNo);

      foreach (var linedetail in existingOrder.OrderDetails.Where(o => o.LineNo == maxLineNo))
      {
        result.OrderLineDetails.Add(new OrderLineDetailModel
        {
          OrderId = linedetail.OrderId,
          ItemDescription = linedetail.ItemDescription,
          UnitPrice = linedetail.UnitPrice,
          Quantity = linedetail.Quantity,
          Discount = linedetail.Discount,
          LineTotal = linedetail.LineTotal
        });
      }

      return result;
    }

    [HttpPost, Route("orderlinedetail/get")]
    public List<OrderLineDetailModel> GetOrderLineDetails([FromBody]GetOrderLineDetailsRequestModel requestModel)
    {
      var existingOrder = Context.Orders.Include(o => o.OrderDetails).Single(o => o.OrderId == requestModel.OrderId);

      var result = new List<OrderLineDetailModel>();

      var maxLineNo = existingOrder.OrderDetails.Max(o => o.LineNo);

      foreach (var linedetail in existingOrder.OrderDetails.Where(o => o.LineNo == maxLineNo))
      {
        result.Add(new OrderLineDetailModel
        {
          OrderId = linedetail.OrderId,
          ItemDescription = linedetail.ItemDescription,
          UnitPrice = linedetail.UnitPrice,
          Quantity = linedetail.Quantity,
          Discount = linedetail.Discount,
          LineTotal = linedetail.LineTotal
        });
      }

      return result;
    }

    [HttpPost, Route("editorderno")]
    public string EditOrderNo([FromBody]EditOrderNoRequestModel requestModel)
    {
      var order = Context.Orders.Single(o => o.OrderId == requestModel.OrderId);

      order.UpdateDate = DateTime.Now;
      order.UpdateUser = requestModel.Username;
      order.OrderNo = requestModel.OrderNo;

      Context.SaveChanges();

      return "Success";
    }

    [HttpPost, Route("customers/get")]
    public List<CustomerModel> GetOrderCustomers([FromBody]GetOrderCustomersRequestModel requestModel)
    {
      var customers = new List<CustomerModel>();

      var userCustomers = Context.Customers.Where(o => o.CreateUser == requestModel.Username).Select(o => new CustomerModel
      {
        CustomerId = o.CustomerId,
        CustomerName = o.CustomerName,
        CompanyProfileId = o.CompanyProfileId,
        CustomerDetails = o.CustomerDetails,
        AccountNo = o.AccountNo,
        ContactNo = o.ContactNo,
        EmailAddress = o.EmailAddress,
        MobileNo = o.MobileNo,
        CreateDate = o.CreateDate,
        CreateUser = o.CreateUser
      }).ToList();

      if (userCustomers != null) customers.AddRange(userCustomers);

      if (requestModel.CompanyProfileId != 0)
      {
        var companyCustomers = Context.Customers.Where(o => o.CreateUser != requestModel.Username && o.CompanyProfileId == requestModel.CompanyProfileId).Select(o => new CustomerModel
        {
          CustomerId = o.CustomerId,
          CustomerName = o.CustomerName,
          CompanyProfileId = o.CompanyProfileId,
          CustomerDetails = o.CustomerDetails,
          AccountNo = o.AccountNo,
          ContactNo = o.ContactNo,
          EmailAddress = o.EmailAddress,
          MobileNo = o.MobileNo,
          CreateDate = o.CreateDate,
          CreateUser = o.CreateUser
        }).ToList();

        if (companyCustomers != null) customers.AddRange(companyCustomers);
      }

      return customers;
    }

    [HttpPost, Route("customers/contacts/get")]
    public List<ContactModel> GetCustomerContacts([FromBody]GetCustomerContactsRequestModel requestModel)
    {
      var contacts = new List<ContactModel>();

      var customer = Context.Customers.Include(o => o.ContactPeople).Single(o => o.CustomerId == requestModel.CustomerId);

      foreach(var contact in customer.ContactPeople)
      {
        contacts.Add(new ContactModel
        {
          ContactId = contact.ContactId,
          ContactName = contact.ContactName,
          ContactNo = contact.ContactNo,
          EmailAddress = contact.EmailAddress,
          CustomerId = contact.CustomerId,
          CreateDate = contact.CreateDate,
          CreateUser = contact.CreateUser
        });
      }

      return contacts;
    }

    [HttpPost, Route("customers/add")]
    public AddCustomerOrderModel AddCustomerOrder([FromBody]AddOrderCustomerRequestModel requestModel)
    {
      var orderHead = Context.Orders.First(o => o.OrderId == requestModel.OrderId);

      var contact = new ContactPerson
      {
        ContactName = requestModel.ContactName,
        ContactNo = requestModel.ContactNo,
        EmailAddress = requestModel.ContactEmailAddress,
        CreateDate = DateTime.Now,
        CreateUser = requestModel.Username,
      };

      var existingCustomer = Context.Customers.Include(cp => cp.ContactPeople).FirstOrDefault(o => o.CustomerId == requestModel.CustomerId);

      if(existingCustomer == null)
      {
        var newCustomer = new Customer
        {
          CustomerName = requestModel.CustomerName,
          CustomerDetails = requestModel.CustomerDetails,
          AccountNo = requestModel.CustomerAccountNo,
          ContactNo = requestModel.CustomerContactNo,
          EmailAddress = requestModel.CustomerEmailAddress,
          MobileNo = requestModel.CustomerMobileNo,
          CompanyProfileId = requestModel.CompanyProfileId,
          CreateDate = DateTime.Now,
          CreateUser = requestModel.Username,
        };

        if (requestModel.ContactAdded)
          newCustomer.ContactPeople.Add(contact);

        Context.Customers.Add(newCustomer);
        Context.SaveChanges();

        orderHead.CustomerId = newCustomer.CustomerId;
        orderHead.UpdateDate = DateTime.Now;
        orderHead.UpdateUser = requestModel.Username;

        if (requestModel.ContactAdded)
          orderHead.ContactId = contact.ContactId;

        Context.SaveChanges();
      }
      else{
        var existingContact = existingCustomer.ContactPeople.FirstOrDefault(cp => cp.ContactId == requestModel.ContactId);

        existingCustomer.CustomerName = requestModel.CustomerName;
        existingCustomer.CustomerDetails = requestModel.CustomerDetails;
        existingCustomer.ContactNo = requestModel.CustomerContactNo;
        existingCustomer.MobileNo = requestModel.CustomerMobileNo;
        existingCustomer.EmailAddress = requestModel.CustomerEmailAddress;
        existingCustomer.AccountNo = requestModel.CustomerAccountNo;
        existingCustomer.UpdateDate = DateTime.Now;
        existingCustomer.UpdateUser = requestModel.Username;

        if (requestModel.ContactAdded && existingContact == null)
          existingCustomer.ContactPeople.Add(contact);
        else {
          if (existingContact != null) {
            existingContact.ContactName = contact.ContactName;
            existingContact.ContactNo = contact.ContactNo;
            existingContact.EmailAddress = contact.EmailAddress;
            existingContact.UpdateDate = DateTime.Now;
            existingContact.UpdateUser = requestModel.Username;
          }          
        }

        orderHead.CustomerId = existingCustomer.CustomerId;
        orderHead.UpdateDate = DateTime.Now;
        orderHead.UpdateUser = requestModel.Username;
        Context.SaveChanges();

        if (requestModel.ContactAdded && existingContact == null)
          orderHead.ContactId = contact.ContactId;
        else
        {
          if (existingContact != null)
            orderHead.ContactId = existingContact.ContactId;
        }

        Context.SaveChanges();
      }

      return new AddCustomerOrderModel
      {
        OrderId = orderHead.OrderId,
        CustomerId = orderHead.CustomerId,
        ContactId = orderHead.ContactId,
        OrderNo = orderHead.OrderNo
      };
    }

    [HttpPost, Route("customer/get")]
    public OrderCustomerDetailModel GetOrderCustomerDetails([FromBody]GetOrderCustomerDetailRequestModel requestModel)
    {
      var orderHead = Context.Orders.First(o => o.OrderId == requestModel.OrderId);

      var existingCustomer = Context.Customers.Include(cp => cp.ContactPeople).FirstOrDefault(o => o.CustomerId == orderHead.CustomerId);

      var existingContact = existingCustomer.ContactPeople.FirstOrDefault(cp => cp.ContactId == orderHead.ContactId);

      return new OrderCustomerDetailModel
      {
        CustomerId = existingCustomer.CustomerId,
        CustomerName = existingCustomer.CustomerName,
        CustomerDetails = existingCustomer.CustomerDetails,
        CustomerContactNo = existingCustomer.ContactNo,
        CustomerAccountNo = existingCustomer.AccountNo,
        CustomerMobileNo = existingCustomer.MobileNo,
        CustomerEmailAddress = existingCustomer.EmailAddress,
        ContactId = orderHead.ContactId,
        ContactAdded = existingContact != null ? true : false,
        ContactName = existingContact != null ? existingContact.ContactName : string.Empty,
        ContactNo = existingContact != null ? existingContact.ContactNo : string.Empty,
        ContactEmailAddress = existingContact != null ? existingContact.EmailAddress ?? " " : string.Empty,
        OrderId = orderHead.OrderId,
        OrderNo = orderHead.OrderNo,
        OrderCreateDate = orderHead.CreateDate
      };
    }

    [HttpPost, Route("customer/address/getall")]
    public List<AddressDetailsModel> GetCustomerAddresses([FromBody]GetCustomerAddressesRequestModel requestModel)
    {
      return Context.Customers.Include(c => c.Addresses).First(c => c.CustomerId == requestModel.CustomerId)
        .Addresses.Select(model => new AddressDetailsModel
        {
          AddressDetailId = model.AddressDetailId,
          AddressLine1 = model.AddressLine1,
          AddressLine2= model.AddressLine2,
          AddressType = model.AddressType,
          City = model.City,
          Country = model.Country,
          PostalCode = model.PostalCode,
          CreateDate = model.CreateDate,
          CreateUser = model.CreateUser
        }).ToList();

    }

    [HttpPost, Route("customer/address/add")]
    public string AddCustomerOrderAddress([FromBody]AddCustomerOrderAddressRequestModel requestModel)
    {
      var customer = Context.Customers.Include(c => c.Addresses).First(c => c.CustomerId == requestModel.CustomerId);
      var order = Context.Orders.First(o => o.OrderId == requestModel.OrderId);

      if (requestModel.AddressDetailId == 0)
      {
        var newAddressDetail = new AddressDetail
        {
          AddressType = requestModel.AddressType,
          AddressLine1 = requestModel.AddressLine1,
          AddressLine2 = requestModel.AddressLine2,
          City = requestModel.City,
          Country = requestModel.Country,
          PostalCode = requestModel.PostalCode,
          CreateDate = DateTime.Now,
          CreateUser = requestModel.Username
        };

        customer.Addresses.Add(newAddressDetail);

        Context.SaveChanges();

        order.AddressDetailId = newAddressDetail.AddressDetailId;
        order.UpdateDate = DateTime.Now;
        order.UpdateUser = requestModel.Username;

        Context.SaveChanges();
      }
      else
      {
        var address = customer.Addresses.First(a => a.AddressDetailId == requestModel.AddressDetailId);
        address.AddressType = requestModel.AddressType;
        address.AddressLine1 = requestModel.AddressLine1;
        address.AddressLine2 = requestModel.AddressLine2;
        address.City = requestModel.City;
        address.Country = requestModel.Country;
        address.PostalCode = requestModel.PostalCode;
        address.UpdateDate = DateTime.Now;
        address.UpdateUser = requestModel.Username;

        order.AddressDetailId = address.AddressDetailId;
        order.UpdateDate = DateTime.Now;
        order.UpdateUser = requestModel.Username;

        Context.SaveChanges();
      }

      return "Success";
    }

    [HttpPost, Route("customer/address/remove")]
    public string RemoveCustomerOrderAddress([FromBody] RemoveCustomerAddressRequestModel requestModel)
    {
      var customer = Context.Customers.Include(c => c.Addresses).First(c => c.CustomerId == requestModel.CustomerId);
      customer.Addresses.RemoveAll(a => a.AddressDetailId == requestModel.AddressDetailId);

      var order = Context.Orders.First(o => o.AddressDetailId == requestModel.AddressDetailId);
      order.AddressDetailId = 0;
      order.UpdateDate = DateTime.Now;
      order.UpdateUser = requestModel.Username;

      Context.SaveChanges();

      return "Success";
    }

    [HttpPost, Route("customer/address/get")]
    public AddressDetailsModel GetCustomerOrderAddress([FromBody]GetCustomerOrderAddressRequestModel requestModel)
    {
      var order = Context.Orders.First(o => o.OrderId == requestModel.OrderId);
        
      return Context.Customers.Include(c => c.Addresses).First(c => c.CustomerId == requestModel.CustomerId).Addresses.Select(model => new AddressDetailsModel
        {
          AddressDetailId = model.AddressDetailId,
          AddressLine1 = model.AddressLine1,
          AddressLine2 = model.AddressLine2,
          AddressType = model.AddressType,
          City = model.City,
          Country = model.Country,
          PostalCode = model.PostalCode,
          CreateDate = model.CreateDate,
          CreateUser = model.CreateUser
        }).First(a => a.AddressDetailId == order.AddressDetailId);

    }
  }
}
