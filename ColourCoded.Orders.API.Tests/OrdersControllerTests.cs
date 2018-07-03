using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using ColourCoded.Orders.API.Models.RequestModels.Orders;
using ColourCoded.Orders.API.Controllers;
using ColourCoded.Orders.API.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ColourCoded.Orders.API.Models.ResponseModels;

namespace ColourCoded.Orders.API.Tests
{
  [TestClass]
  public class OrdersControllerTests
  {
    private class Resources
    {
      public OrdersController Controller;
      public OrdersContext Context;
      public IConfiguration Configuration;
      public MemoryCache MemoryCache;
      public string TestUsername { get; set; }
      public int CompanyProfileId { get; set; }

      public Resources()
      {
        TestUsername = "testuser";
        CompanyProfileId = 1;
        Configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

        MemoryCache = new MemoryCache(new MemoryCacheOptions());
        Context = TestHelper.CreateDbContext(Configuration);
        Controller = new OrdersController(Context);
      }

    }

    [TestMethod]
    public void GetHomeOrders_NoCompany()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        const string username = "testuser";

        var orderOne = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST001", salesPerson: username, salesDateAddMonths: -1);
        var orderTwo = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST002", salesPerson: username, salesDateAddMonths: -2);
        var orderThree = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST003", salesPerson: "differentUser", salesDateAddMonths: -3);

        var requestModel = new GetHomeOrdersRequestModel { Username = username, CompanyProfileId = 0 };

        // When
        var result = resources.Controller.GetHomeOrders(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(orderOne.OrderNo, result[0].OrderNo);
        Assert.AreEqual(orderOne.OrderId, result[0].OrderId);
        Assert.AreEqual(orderOne.OrderTotal.ToString("R 0 000.00"), result[0].Total);

        Assert.AreEqual(orderTwo.OrderNo, result[1].OrderNo);
        Assert.AreEqual(orderTwo.OrderId, result[1].OrderId);
        Assert.AreEqual(orderTwo.OrderTotal.ToString("R 0 000.00"), result[1].Total);
      }
    }

    [TestMethod]
    public void GetHomeOrders_HasCompanyProfile()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        const string username = "testuser";

        TestHelper.RemoveOrderHeads(resources.Context);
        var company = TestHelper.CreateCompany(resources.Context);
        var orderOne = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST001", salesPerson: username, salesDateAddMonths: -1);
        var orderTwo = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST002", salesPerson: "differentUser", salesDateAddMonths: -2, companyProfileId: company.CompanyProfileId);
        var orderThree = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST003", salesPerson: "anotherUser", salesDateAddMonths: -3);

        var requestModel = new GetHomeOrdersRequestModel { Username = username, CompanyProfileId = company.CompanyProfileId };

        // When
        var result = resources.Controller.GetHomeOrders(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(orderOne.OrderNo, result[0].OrderNo);
        Assert.AreEqual(orderOne.OrderId, result[0].OrderId);
        Assert.AreEqual(orderOne.OrderTotal.ToString("R 0 000.00"), result[0].Total);

        Assert.AreEqual(orderTwo.OrderNo, result[1].OrderNo);
        Assert.AreEqual(orderTwo.OrderId, result[1].OrderId);
        Assert.AreEqual(orderTwo.OrderTotal.ToString("R 0 000.00"), result[1].Total);
      }
    }

    [TestMethod]
    public void GetHomeOrdersByPeriod_NoCompany()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        const string username = "testuser";

        TestHelper.RemoveOrderHeads(resources.Context);
        var orderOne = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST001", salesPerson: username, salesDateAddMonths: -1);
        var orderTwo = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST002", salesPerson: username, salesDateAddMonths: -15);
        var orderThree = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST003", salesPerson: "differentUser", salesDateAddMonths: -18);
        var orderFour = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST004", salesPerson: username, salesDateAddMonths: -30);

        var requestModel = new GetHomeOrdersPeriodRequestModel { Username = username, CompanyProfileId = 0, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now };

        // When
        var result = resources.Controller.GetHomeOrdersInPeriod(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(orderOne.OrderNo, result[0].OrderNo);
        Assert.AreEqual(orderOne.OrderId, result[0].OrderId);
        Assert.AreEqual(orderOne.OrderTotal.ToString("R 0 000.00"), result[0].Total);

        Assert.AreEqual(orderTwo.OrderNo, result[1].OrderNo);
        Assert.AreEqual(orderTwo.OrderId, result[1].OrderId);
        Assert.AreEqual(orderTwo.OrderTotal.ToString("R 0 000.00"), result[1].Total);
      }
    }

    [TestMethod]
    public void GetHomeOrdersByPeriod_HasCompany()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        const string username = "testuser";

        TestHelper.RemoveOrderHeads(resources.Context);
        var company = TestHelper.CreateCompany(resources.Context);
        var orderOne = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST001", salesPerson: username, salesDateAddMonths: -1);
        var orderTwo = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST002", salesPerson: username, salesDateAddMonths: -15);
        var orderThree = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST003", salesPerson: "differentUser", salesDateAddMonths: -18, companyProfileId: company.CompanyProfileId);
        var orderFour = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST004", salesPerson: username, salesDateAddMonths: -30, companyProfileId: company.CompanyProfileId);

        var requestModel = new GetHomeOrdersPeriodRequestModel { Username = username, CompanyProfileId = company.CompanyProfileId, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now };

        // When
        var result = resources.Controller.GetHomeOrdersInPeriod(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);

        Assert.AreEqual(orderOne.OrderNo, result[0].OrderNo);
        Assert.AreEqual(orderOne.OrderId, result[0].OrderId);
        Assert.AreEqual(orderOne.OrderTotal.ToString("R 0 000.00"), result[0].Total);

        Assert.AreEqual(orderTwo.OrderNo, result[1].OrderNo);
        Assert.AreEqual(orderTwo.OrderId, result[1].OrderId);
        Assert.AreEqual(orderTwo.OrderTotal.ToString("R 0 000.00"), result[1].Total);

        Assert.AreEqual(orderThree.OrderNo, result[2].OrderNo);
        Assert.AreEqual(orderThree.OrderId, result[2].OrderId);
        Assert.AreEqual(orderThree.OrderTotal.ToString("R 0 000.00"), result[2].Total);
      }
    }

    [TestMethod]
    public void AddOrderHead_Success_ReturnsInt()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        const string orderNo = "TEST123";
        var company = TestHelper.CreateCompany(resources.Context);
        var requestModel = new AddOrderRequestModel { OrderNo = orderNo, Username = resources.TestUsername, CompanyProfileId = company.CompanyProfileId};
        var oldSeed = company.OrderNoSeed;

        // when
        var result = resources.Controller.AddOrder(requestModel);

        // then
        Assert.IsNotNull(result);

        var savedOrderHead = resources.Context.Orders.First(o => o.OrderNo == orderNo);
        Assert.AreEqual(requestModel.Username, savedOrderHead.CreateUser);
        Assert.AreEqual(0, savedOrderHead.SubTotal);
        Assert.AreEqual(0, savedOrderHead.VatTotal);
        Assert.AreEqual(0, savedOrderHead.DiscountTotal);
        Assert.AreEqual(0, savedOrderHead.OrderTotal);
        Assert.AreEqual(company.CompanyProfileId, savedOrderHead.CompanyProfileId);
        Assert.IsNull(savedOrderHead.UpdateDate);
        Assert.IsNull(savedOrderHead.UpdateUser);
        Assert.IsNotNull(savedOrderHead.OrderId);
        Assert.IsNotNull(savedOrderHead.CreateDate);

        Assert.AreEqual(oldSeed + 1, company.OrderNoSeed);
      }
    }

    [TestMethod]
    public void AddOrderHead_AlreadyExists_ReturnsInt()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        const string orderNo = "TEST123";
        var requestModel = new AddOrderRequestModel { OrderNo = orderNo, Username = resources.TestUsername };
        var order = TestHelper.CreateOrderHead(resources.Context, orderNo, resources.TestUsername, 0M, 0M);

        // when
        var result = resources.Controller.AddOrder(requestModel);

        // then
        Assert.IsNotNull(result);
        var savedOrderHead = resources.Context.Orders.Single(o => o.OrderNo == orderNo);
        Assert.AreEqual(requestModel.Username, savedOrderHead.CreateUser);
        Assert.AreEqual(0, savedOrderHead.SubTotal);
        Assert.AreEqual(0, savedOrderHead.VatTotal);
        Assert.AreEqual(0, savedOrderHead.DiscountTotal);
        Assert.AreEqual(0, savedOrderHead.OrderTotal);
        Assert.IsNull(savedOrderHead.UpdateDate);
        Assert.IsNull(savedOrderHead.UpdateUser);
        Assert.IsNotNull(savedOrderHead.OrderId);
        Assert.IsNotNull(savedOrderHead.CreateDate);
      }
    }

    [TestMethod]
    public void AddOrdeDetail_Success_ReturnsInt()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var requestModel = new List<AddOrderDetailRequestModel>();

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Pizza",
          UnitPrice = 100M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 200M,
          Username = resources.TestUsername
        });

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Delivery Fee",
          UnitPrice = 50M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 100M,
          Username = resources.TestUsername
        });

        // when
        var result = resources.Controller.AddOrderDetail(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
        var savedOrderDetail = resources.Context.Orders.Include(o => o.OrderDetails).First(o => o.OrderId == orderHead.OrderId);
        Assert.IsNotNull(savedOrderDetail.OrderDetails);
        Assert.AreEqual(2, savedOrderDetail.OrderDetails.Count);

        Assert.IsTrue(savedOrderDetail.OrderDetails[0].LineNo == 1);
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].OrderId == orderHead.OrderId);
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].ItemDescription == "Pizza");
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].UnitPrice == 100M);
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].Quantity == 2);
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].Discount == 0M);
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].LineTotal == 200M);
        Assert.IsTrue(savedOrderDetail.OrderDetails[0].CreateUser == resources.TestUsername);

        Assert.IsTrue(savedOrderDetail.OrderDetails[1].LineNo == 1);
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].OrderId == orderHead.OrderId);
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].ItemDescription == "Delivery Fee");
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].UnitPrice == 50M);
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].Quantity == 2);
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].Discount == 0M);
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].LineTotal == 100M);
        Assert.IsTrue(savedOrderDetail.OrderDetails[1].CreateUser == resources.TestUsername);
      }
    }

    [TestMethod]
    public void AddOrderDetail_LessThanZero_Invalid()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var requestModel = new List<AddOrderDetailRequestModel>();

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Pizza",
          UnitPrice = 100M,
          Quantity = -2,
          Discount = 0M,
          LineTotal = -200M,
          Username = resources.TestUsername
        });

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Delivery Fee",
          UnitPrice = 50M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 100M,
          Username = resources.TestUsername
        });

        // when
        var result = resources.Controller.AddOrderDetail(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("No values may be less than zero", result.Messages[0].Message);
        var savedOrderDetail = resources.Context.Orders.Include(o => o.OrderDetails).First(o => o.OrderId == orderHead.OrderId);
        Assert.IsNotNull(savedOrderDetail.OrderDetails);
        Assert.AreEqual(0, savedOrderDetail.OrderDetails.Count);

      }
    }

    [TestMethod]
    public void AddOrderDetail_IncorrectLineTotal_Invalid()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var requestModel = new List<AddOrderDetailRequestModel>();

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Pizza",
          UnitPrice = 100M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 230M,
          Username = resources.TestUsername
        });

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Delivery Fee",
          UnitPrice = 50M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 50M,
          Username = resources.TestUsername
        });

        // when
        var result = resources.Controller.AddOrderDetail(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Incorrect line total", result.Messages[0].Message);
        var savedOrderDetail = resources.Context.Orders.Include(o => o.OrderDetails).First(o => o.OrderId == orderHead.OrderId);
        Assert.IsNotNull(savedOrderDetail.OrderDetails);
        Assert.AreEqual(0, savedOrderDetail.OrderDetails.Count);

      }
    }

    [TestMethod]
    public void AddOrdeDetail_IncorrectOrderId_Invalid()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var requestModel = new List<AddOrderDetailRequestModel>();

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderHead.OrderId,
          ItemDescription = "Pizza",
          UnitPrice = 100M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 200M,
          Username = resources.TestUsername
        });

        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = 2341234,
          ItemDescription = "Delivery Fee",
          UnitPrice = 50M,
          Quantity = 2,
          Discount = 0M,
          LineTotal = 100M,
          Username = resources.TestUsername
        });

        // when
        var result = resources.Controller.AddOrderDetail(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Not all Order Id's match.", result.Messages[0].Message);
        var savedOrderDetail = resources.Context.Orders.Include(o => o.OrderDetails).First(o => o.OrderId == orderHead.OrderId);
        Assert.IsNotNull(savedOrderDetail.OrderDetails);
        Assert.AreEqual(0, savedOrderDetail.OrderDetails.Count);

      }
    }

    [TestMethod]
    public void GetOrderDetailLineNo_ReturnsInt()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var order = TestHelper.CreateOrderHead(resources.Context);

        var requestModel = new GetOrderDetailLineNoRequestModel { OrderId = order.OrderId };

        // when
        var result = resources.Controller.GetOrderLineNo(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result);
      }
    }

    [TestMethod]
    public void GetOrderDetail_ReturnsOrderDetailModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var orderDetailOne = TestHelper.CreateOrderDetail(resources.Context, orderHead);
        var orderDetailTwo = TestHelper.CreateOrderDetail(resources.Context, orderHead);

        var requestModel = new GetOrderDetailRequestModel { OrderId = orderHead.OrderId };

        // When
        var result = resources.Controller.GetOrderDetail(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.OrderLineDetails.Count);

        Assert.AreEqual(orderDetailOne.ItemDescription, result.OrderLineDetails[0].ItemDescription);
        Assert.AreEqual(orderDetailOne.UnitPrice, result.OrderLineDetails[0].UnitPrice);
        Assert.AreEqual(orderDetailOne.Quantity, result.OrderLineDetails[0].Quantity);
        Assert.AreEqual(orderDetailOne.LineTotal, result.OrderLineDetails[0].LineTotal);
        Assert.AreEqual(orderDetailOne.Discount, result.OrderLineDetails[0].Discount);

        Assert.AreEqual(orderDetailOne.ItemDescription, result.OrderLineDetails[1].ItemDescription);
        Assert.AreEqual(orderDetailOne.UnitPrice, result.OrderLineDetails[1].UnitPrice);
        Assert.AreEqual(orderDetailOne.Quantity, result.OrderLineDetails[1].Quantity);
        Assert.AreEqual(orderDetailOne.LineTotal, result.OrderLineDetails[1].LineTotal);
        Assert.AreEqual(orderDetailOne.Discount, result.OrderLineDetails[1].Discount);
      }
    }

    [TestMethod]
    public void GetOrderLineDetails_ReturnsOrderLineDetailModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var orderDetailOne = TestHelper.CreateOrderDetail(resources.Context, orderHead);
        var orderDetailTwo = TestHelper.CreateOrderDetail(resources.Context, orderHead);

        var requestModel = new GetOrderLineDetailsRequestModel { OrderId = orderHead.OrderId };

        // When 
        var result = resources.Controller.GetOrderLineDetails(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(orderDetailOne.ItemDescription, result[0].ItemDescription);
        Assert.AreEqual(orderDetailOne.UnitPrice, result[0].UnitPrice);
        Assert.AreEqual(orderDetailOne.Quantity, result[0].Quantity);
        Assert.AreEqual(orderDetailOne.LineTotal, result[0].LineTotal);
        Assert.AreEqual(orderDetailOne.Discount, result[0].Discount);

        Assert.AreEqual(orderDetailOne.ItemDescription, result[1].ItemDescription);
        Assert.AreEqual(orderDetailOne.UnitPrice, result[1].UnitPrice);
        Assert.AreEqual(orderDetailOne.Quantity, result[1].Quantity);
        Assert.AreEqual(orderDetailOne.LineTotal, result[1].LineTotal);
        Assert.AreEqual(orderDetailOne.Discount, result[1].Discount);
      }
    }

    [TestMethod]
    public void EditOrderNo_ReturnsSuccess()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var order = TestHelper.CreateOrderHead(resources.Context);

        var requestModel = new EditOrderNoRequestModel { OrderId = order.OrderId, OrderNo = "TEST123", Username = resources.TestUsername };

        // when
        var result = resources.Controller.EditOrderNo(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual("Success", result);
      }
    }

    [TestMethod]
    public void GetVatRate_ReturnsDecimal()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        const decimal vatRate = 0.15M;
        TestHelper.RemoveTaxRates(resources.Context);
        var vatTax = TestHelper.CreateTaxRate(resources.Context, vatRate, OrdersConstants.VatTaxCode);

        // when
        var result = resources.Controller.GetVatRate();

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(vatRate, result);
      }
    }

    [TestMethod]
    public void GetOrderNoSeed_NoCompany()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        const int orderNoSeed = 122;
        var company = TestHelper.CreateCompany(resources.Context, orderNoSeed);
        var requestModel = new GetCompanyOrderNoSeedRequestModel { CompanyProfileId = 0 };

        // when
        var result = resources.Controller.GetOrderNoSeed(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result);
      }
    }

    [TestMethod]
    public void GetOrderNoSeed_HasCompany()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        const int orderNoSeed = 122;
        var company = TestHelper.CreateCompany(resources.Context, orderNoSeed);
        var requestModel = new GetCompanyOrderNoSeedRequestModel { CompanyProfileId = company.CompanyProfileId };

        // when
        var result = resources.Controller.GetOrderNoSeed(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(orderNoSeed, result);
      }
    }

    [TestMethod]
    public void GetOrderCustomers_LinkedToCompanyProfile_User_ReturnsCustomerModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var customerOne = TestHelper.CreateCustomer(resources.Context, companyProfileId: 0, username: resources.TestUsername);
        var customerTwo = TestHelper.CreateCustomer(resources.Context, companyProfileId: resources.CompanyProfileId);
        var customerThree = TestHelper.CreateCustomer(resources.Context, companyProfileId: resources.CompanyProfileId);

        var customerFour = TestHelper.CreateCustomer(resources.Context, companyProfileId: 999);
        var customerFive = TestHelper.CreateCustomer(resources.Context, username: "otheruser");

        var requestModel = new GetOrderCustomersRequestModel { CompanyProfileId = resources.CompanyProfileId, Username = resources.TestUsername };

        // when
        var result = resources.Controller.GetOrderCustomers(requestModel) as List<CustomerModel>;

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);

        Assert.AreEqual(customerOne.CustomerName, result[0].CustomerName);
        Assert.AreEqual(customerOne.CustomerId, result[0].CustomerId);
        Assert.AreEqual(customerOne.CustomerDetails, result[0].CustomerDetails);
        Assert.AreEqual(customerOne.ContactNo, result[0].ContactNo);
        Assert.AreEqual(customerOne.AccountNo, result[0].AccountNo);
        Assert.AreEqual(customerOne.CompanyProfileId, result[0].CompanyProfileId);
        Assert.AreEqual(customerOne.EmailAddress, result[0].EmailAddress);

        Assert.AreEqual(customerTwo.CustomerName, result[1].CustomerName);
        Assert.AreEqual(customerTwo.CustomerId, result[1].CustomerId);
        Assert.AreEqual(customerTwo.CustomerDetails, result[1].CustomerDetails);
        Assert.AreEqual(customerTwo.ContactNo, result[1].ContactNo);
        Assert.AreEqual(customerTwo.AccountNo, result[1].AccountNo);
        Assert.AreEqual(customerTwo.CompanyProfileId, result[1].CompanyProfileId);
        Assert.AreEqual(customerTwo.EmailAddress, result[1].EmailAddress);

        Assert.AreEqual(customerThree.CustomerName, result[2].CustomerName);
        Assert.AreEqual(customerThree.CustomerId, result[2].CustomerId);
        Assert.AreEqual(customerThree.CustomerDetails, result[2].CustomerDetails);
        Assert.AreEqual(customerThree.ContactNo, result[2].ContactNo);
        Assert.AreEqual(customerThree.AccountNo, result[2].AccountNo);
        Assert.AreEqual(customerThree.CompanyProfileId, result[2].CompanyProfileId);
        Assert.AreEqual(customerThree.EmailAddress, result[2].EmailAddress);
      }

    }

    [TestMethod]
    public void GetCustomerContacts()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var customerOne = TestHelper.CreateCustomer(resources.Context, companyProfileId: 0, username: resources.TestUsername);
        var contactOne = TestHelper.CreateContactPerson(resources.Context, customerOne);
        var contactTwo = TestHelper.CreateContactPerson(resources.Context, customerOne);

        var customerTwo = TestHelper.CreateCustomer(resources.Context, companyProfileId: resources.CompanyProfileId);
        var contactThree = TestHelper.CreateContactPerson(resources.Context, customerTwo);

        var requestModel = new GetCustomerContactsRequestModel { CustomerId = customerOne.CustomerId };

        // when
        var result = resources.Controller.GetCustomerContacts(requestModel) as List<ContactModel>;

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(contactOne.ContactName, result[0].ContactName);
        Assert.AreEqual(contactOne.ContactId, result[0].ContactId);
        Assert.AreEqual(contactOne.ContactNo, result[0].ContactNo);
        Assert.AreEqual(contactOne.EmailAddress, result[0].EmailAddress);
        Assert.AreEqual(contactOne.CustomerId, result[0].CustomerId);

        Assert.AreEqual(contactTwo.ContactName, result[1].ContactName);
        Assert.AreEqual(contactTwo.ContactId, result[1].ContactId);
        Assert.AreEqual(contactTwo.ContactNo, result[1].ContactNo);
        Assert.AreEqual(contactTwo.EmailAddress, result[1].EmailAddress);
        Assert.AreEqual(contactTwo.CustomerId, result[1].CustomerId);

      }

    }

    [TestMethod]
    public void AddOrder_NewCustomerNewContact_ReturnsAddOrderCustomerModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);

        var requestModel = new AddOrderCustomerRequestModel
        {
          OrderId = orderHead.OrderId,
          CustomerId = 0,
          CustomerName = "TestCustomer",
          CustomerDetails = "Some Long Customer Description",
          CustomerContactNo = "0214475588",
          CustomerMobileNo = "0425584477",
          CustomerAccountNo = "DE1234",
          CustomerEmailAddress = "someemail@gmail.com",
          ContactId = 0,
          ContactAdded = true,
          ContactEmailAddress = "contact@gmail.com",
          ContactName = "ziziu",
          ContactNo = "0218827777",
          Username = resources.TestUsername,
          CompanyProfileId = resources.CompanyProfileId
        };

        // when
        var result = resources.Controller.AddCustomerOrder(requestModel);

        // then
        Assert.IsNotNull(result);
        var saveCustomerDetail = resources.Context.Customers.Include(o => o.ContactPeople).First(o => o.CustomerId == orderHead.CustomerId);
        Assert.IsNotNull(saveCustomerDetail);
        Assert.IsNotNull(saveCustomerDetail.ContactPeople);
        Assert.AreEqual(1, saveCustomerDetail.ContactPeople.Count);

        Assert.IsTrue(saveCustomerDetail.AccountNo == requestModel.CustomerAccountNo);
        Assert.IsTrue(saveCustomerDetail.CompanyProfileId == resources.CompanyProfileId);
        Assert.IsTrue(saveCustomerDetail.CreateUser == resources.TestUsername);
        Assert.IsTrue(saveCustomerDetail.ContactNo == requestModel.CustomerContactNo);
        Assert.IsTrue(saveCustomerDetail.CustomerDetails == requestModel.CustomerDetails);
        Assert.IsTrue(saveCustomerDetail.CustomerName == requestModel.CustomerName);
        Assert.IsTrue(saveCustomerDetail.MobileNo == requestModel.CustomerMobileNo);
        Assert.IsTrue(saveCustomerDetail.EmailAddress == requestModel.CustomerEmailAddress);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].ContactName == requestModel.ContactName);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].ContactNo == requestModel.ContactNo);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].EmailAddress == requestModel.ContactEmailAddress);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].CreateUser == resources.TestUsername);

        Assert.AreEqual(saveCustomerDetail.CustomerId, result.CustomerId);
        Assert.AreEqual(saveCustomerDetail.ContactPeople[0].CustomerId, result.ContactId);
        Assert.AreEqual(orderHead.OrderId, result.OrderId);
        Assert.AreEqual(orderHead.OrderNo, result.OrderNo);
      }
    }

    [TestMethod]
    public void AddOrder_NewCustomerNoContact_ReturnsAddOrderCustomerModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);

        var requestModel = new AddOrderCustomerRequestModel
        {
          OrderId = orderHead.OrderId,
          CustomerId = 0,
          CustomerName = "TestCustomer",
          CustomerDetails = "Some Long Customer Description",
          CustomerContactNo = "0214475588",
          CustomerMobileNo = "0425584477",
          CustomerAccountNo = "DE1234",
          CustomerEmailAddress = "someemail@gmail.com",
          ContactId = 0,
          ContactAdded = false,
          Username = resources.TestUsername,
          CompanyProfileId = resources.CompanyProfileId
        };

        // when
        var result = resources.Controller.AddCustomerOrder(requestModel);

        // then
        Assert.IsNotNull(result);
        var saveCustomerDetail = resources.Context.Customers.Include(o => o.ContactPeople).First(o => o.CustomerId == orderHead.CustomerId);
        Assert.IsNotNull(saveCustomerDetail);
        Assert.IsNotNull(saveCustomerDetail.ContactPeople);
        Assert.AreEqual(0, saveCustomerDetail.ContactPeople.Count);

        Assert.IsTrue(saveCustomerDetail.AccountNo == requestModel.CustomerAccountNo);
        Assert.IsTrue(saveCustomerDetail.CompanyProfileId == resources.CompanyProfileId);
        Assert.IsTrue(saveCustomerDetail.CreateUser == resources.TestUsername);
        Assert.IsTrue(saveCustomerDetail.ContactNo == requestModel.CustomerContactNo);
        Assert.IsTrue(saveCustomerDetail.CustomerDetails == requestModel.CustomerDetails);
        Assert.IsTrue(saveCustomerDetail.CustomerName == requestModel.CustomerName);
        Assert.IsTrue(saveCustomerDetail.MobileNo == requestModel.CustomerMobileNo);
        Assert.IsTrue(saveCustomerDetail.EmailAddress == requestModel.CustomerEmailAddress);

        Assert.AreEqual(saveCustomerDetail.CustomerId, result.CustomerId);
        Assert.AreEqual(orderHead.ContactId, result.ContactId);
        Assert.AreEqual(orderHead.OrderId, result.OrderId);
        Assert.AreEqual(orderHead.OrderNo, result.OrderNo);
      }
    }

    [TestMethod]
    public void AddOrder_NewCustomerExistingContact_ReturnsAddOrderCustomerModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var customer = TestHelper.CreateCustomer(resources.Context, companyProfileId: resources.CompanyProfileId);
        var contactPerson = TestHelper.CreateContactPerson(resources.Context, customer);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);

        var requestModel = new AddOrderCustomerRequestModel
        {
          OrderId = orderHead.OrderId,
          CustomerId = customer.CustomerId,
          CustomerName = "TestCustomer",
          CustomerDetails = "Some Long Customer Description",
          CustomerContactNo = "0214475588",
          CustomerMobileNo = "0425584477",
          CustomerAccountNo = "DE1234",
          CustomerEmailAddress = "someemail@gmail.com",
          ContactId = contactPerson.ContactId,
          ContactAdded = true,
          ContactName = "testinganothername",
          ContactEmailAddress = "asdf@gmail.com",
          ContactNo = "123123123",
          Username = resources.TestUsername,
          CompanyProfileId = resources.CompanyProfileId
        };

        // when
        var result = resources.Controller.AddCustomerOrder(requestModel);

        // then
        // then
        Assert.IsNotNull(result);
        var saveCustomerDetail = resources.Context.Customers.Include(o => o.ContactPeople).First(o => o.CustomerId == customer.CustomerId);
        Assert.IsNotNull(saveCustomerDetail);
        Assert.IsNotNull(saveCustomerDetail.ContactPeople);
        Assert.AreEqual(1, saveCustomerDetail.ContactPeople.Count);

        Assert.IsTrue(saveCustomerDetail.AccountNo == requestModel.CustomerAccountNo);
        Assert.IsTrue(saveCustomerDetail.CompanyProfileId == resources.CompanyProfileId);
        Assert.IsTrue(saveCustomerDetail.UpdateUser == resources.TestUsername);
        Assert.IsTrue(saveCustomerDetail.ContactNo == requestModel.CustomerContactNo);
        Assert.IsTrue(saveCustomerDetail.CustomerDetails == requestModel.CustomerDetails);
        Assert.IsTrue(saveCustomerDetail.CustomerName == requestModel.CustomerName);
        Assert.IsTrue(saveCustomerDetail.MobileNo == requestModel.CustomerMobileNo);
        Assert.IsTrue(saveCustomerDetail.EmailAddress == requestModel.CustomerEmailAddress);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].ContactName == requestModel.ContactName);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].ContactNo == requestModel.ContactNo);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].EmailAddress == requestModel.ContactEmailAddress);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].UpdateUser == resources.TestUsername);

        Assert.AreEqual(saveCustomerDetail.CustomerId, result.CustomerId);
        Assert.AreEqual(saveCustomerDetail.ContactPeople[0].CustomerId, result.ContactId);
        Assert.AreEqual(orderHead.OrderId, result.OrderId);
        Assert.AreEqual(orderHead.OrderNo, result.OrderNo);
      }
    }

    [TestMethod]
    public void AddOrder_OldCustomerNewContact_ReturnsAddOrderCustomerModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var customer = TestHelper.CreateCustomer(resources.Context);

        var requestModel = new AddOrderCustomerRequestModel
        {
          OrderId = orderHead.OrderId,
          CustomerId = customer.CustomerId,
          CustomerName = "TestCustomer",
          CustomerDetails = "Some Long Customer Description",
          CustomerContactNo = "0214475588",
          CustomerMobileNo = "0425584477",
          CustomerAccountNo = "DE1234",
          CustomerEmailAddress = "someemail@gmail.com",
          ContactId = 0,
          ContactAdded = true,
          ContactEmailAddress = "contact@gmail.com",
          ContactName = "ziziu",
          ContactNo = "0218827777",
          Username = resources.TestUsername,
          CompanyProfileId = resources.CompanyProfileId
        };

        // when
        var result = resources.Controller.AddCustomerOrder(requestModel);

        // then
        Assert.IsNotNull(result);
        var saveCustomerDetail = resources.Context.Customers.Include(o => o.ContactPeople).First(o => o.CustomerId == customer.CustomerId);
        Assert.IsNotNull(saveCustomerDetail);
        Assert.IsNotNull(saveCustomerDetail.ContactPeople);
        Assert.AreEqual(1, saveCustomerDetail.ContactPeople.Count);

        Assert.IsTrue(saveCustomerDetail.AccountNo == requestModel.CustomerAccountNo);
        Assert.IsTrue(saveCustomerDetail.ContactNo == requestModel.CustomerContactNo);
        Assert.IsTrue(saveCustomerDetail.CustomerDetails == requestModel.CustomerDetails);
        Assert.IsTrue(saveCustomerDetail.CustomerName == requestModel.CustomerName);
        Assert.IsTrue(saveCustomerDetail.MobileNo == requestModel.CustomerMobileNo);
        Assert.IsTrue(saveCustomerDetail.EmailAddress == requestModel.CustomerEmailAddress);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].ContactName == requestModel.ContactName);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].ContactNo == requestModel.ContactNo);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].EmailAddress == requestModel.ContactEmailAddress);
        Assert.IsTrue(saveCustomerDetail.ContactPeople[0].CreateUser == resources.TestUsername);

        Assert.AreEqual(saveCustomerDetail.CustomerId, result.CustomerId);
        Assert.AreEqual(saveCustomerDetail.ContactPeople[0].CustomerId, result.ContactId);
        Assert.AreEqual(orderHead.OrderId, result.OrderId);
        Assert.AreEqual(orderHead.OrderNo, result.OrderNo);
      }
    }

    [TestMethod]
    public void AddOrder_OldCustomerNoContact_ReturnsAddOrderCustomerModel()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var customer = TestHelper.CreateCustomer(resources.Context);

        var requestModel = new AddOrderCustomerRequestModel
        {
          OrderId = orderHead.OrderId,
          CustomerId = customer.CustomerId,
          CustomerName = "TestCustomer",
          CustomerDetails = "Some Long Customer Description",
          CustomerContactNo = "0214475588",
          CustomerMobileNo = "0425584477",
          CustomerAccountNo = "DE1234",
          CustomerEmailAddress = "someemail@gmail.com",
          ContactId = 0,
          ContactAdded = false,
          Username = resources.TestUsername,
          CompanyProfileId = resources.CompanyProfileId
        };

        // when
        var result = resources.Controller.AddCustomerOrder(requestModel);

        // then
        Assert.IsNotNull(result);
        var saveCustomerDetail = resources.Context.Customers.Include(o => o.ContactPeople).First(o => o.CustomerId == customer.CustomerId);
        Assert.IsNotNull(saveCustomerDetail);
        Assert.IsNotNull(saveCustomerDetail.ContactPeople);
        Assert.AreEqual(0, saveCustomerDetail.ContactPeople.Count);

        Assert.IsTrue(saveCustomerDetail.AccountNo == requestModel.CustomerAccountNo);
        Assert.IsTrue(saveCustomerDetail.ContactNo == requestModel.CustomerContactNo);
        Assert.IsTrue(saveCustomerDetail.CustomerDetails == requestModel.CustomerDetails);
        Assert.IsTrue(saveCustomerDetail.CustomerName == requestModel.CustomerName);
        Assert.IsTrue(saveCustomerDetail.MobileNo == requestModel.CustomerMobileNo);
        Assert.IsTrue(saveCustomerDetail.EmailAddress == requestModel.CustomerEmailAddress);

        Assert.AreEqual(saveCustomerDetail.CustomerId, result.CustomerId);
        Assert.AreEqual(orderHead.ContactId, result.ContactId);
        Assert.AreEqual(orderHead.OrderId, result.OrderId);
        Assert.AreEqual(orderHead.OrderNo, result.OrderNo);
      }
    }

    [TestMethod]
    public void GetOrderCustomerDetails_Success()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        TestHelper.RemoveCustomers(resources.Context);
        var customer = TestHelper.CreateCustomer(resources.Context);
        var contact = TestHelper.CreateContactPerson(resources.Context, customer);
        var orderHead = TestHelper.CreateOrderHead(resources.Context, customerId: customer.CustomerId, contactId: contact.ContactId);

        var requestModel = new GetOrderCustomerDetailRequestModel { OrderId = orderHead.OrderId};

        // when
        var result = resources.Controller.GetOrderCustomerDetails(requestModel) as OrderCustomerDetailModel;

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual(customer.CustomerId, result.CustomerId);
        Assert.AreEqual(customer.CustomerName, result.CustomerName);
        Assert.AreEqual(customer.CustomerDetails, result.CustomerDetails);
        Assert.AreEqual(customer.ContactNo, result.CustomerContactNo);
        Assert.AreEqual(customer.EmailAddress, result.CustomerEmailAddress);
        Assert.AreEqual(customer.AccountNo, result.CustomerAccountNo);
        Assert.AreEqual(customer.MobileNo, result.CustomerMobileNo);
        Assert.AreEqual(customer.ContactPeople[0].ContactId, result.ContactId);
        Assert.AreEqual(customer.ContactPeople[0].ContactName, result.ContactName);
        Assert.AreEqual(customer.ContactPeople[0].ContactNo, result.ContactNo);
        Assert.AreEqual(customer.ContactPeople[0].EmailAddress, result.ContactEmailAddress);
      }

    }

    [TestMethod]
    public void GetCustomerAddresses()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var customer = TestHelper.CreateCustomer(resources.Context);
        var addressOne = TestHelper.CreateCustomerAddress(resources.Context, customer);
        var addressTwo = TestHelper.CreateCustomerAddress(resources.Context, customer);
        var requestModel = new GetCustomerAddressesRequestModel { CustomerId = customer.CustomerId };

        // When 
        var result = resources.Controller.GetCustomerAddresses(requestModel);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.Any(o => o.AddressLine1 == addressOne.AddressLine1));
        Assert.IsTrue(result.Any(o => o.AddressLine2 == addressOne.AddressLine2));
        Assert.IsTrue(result.Any(o => o.City == addressOne.City));
        Assert.IsTrue(result.Any(o => o.Country == addressOne.Country));
        Assert.IsTrue(result.Any(o => o.PostalCode == addressOne.PostalCode));

        Assert.IsTrue(result.Any(o => o.AddressLine1 == addressTwo.AddressLine1));
        Assert.IsTrue(result.Any(o => o.AddressLine2 == addressTwo.AddressLine2));
        Assert.IsTrue(result.Any(o => o.City == addressTwo.City));
        Assert.IsTrue(result.Any(o => o.Country == addressTwo.Country));
        Assert.IsTrue(result.Any(o => o.PostalCode == addressTwo.PostalCode));

      }
      
    }

    [TestMethod]
    public void GetCustomerAddress()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var customer = TestHelper.CreateCustomer(resources.Context);
        var addressOne = TestHelper.CreateCustomerAddress(resources.Context, customer);
        var orderHead = TestHelper.CreateOrderHead(resources.Context, addressDetailId: addressOne.AddressDetailId);
        var requestModel = new GetCustomerOrderAddressRequestModel { CustomerId = customer.CustomerId, OrderId = orderHead.OrderId };

        // When 
        var result = resources.Controller.GetCustomerOrderAddress(requestModel) as AddressDetailsModel;

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(result.AddressDetailId, addressOne.AddressDetailId);
        Assert.AreEqual(result.AddressLine1, addressOne.AddressLine1);
        Assert.AreEqual(result.AddressLine2, addressOne.AddressLine2);
        Assert.AreEqual(result.AddressType, addressOne.AddressType);
        Assert.AreEqual(result.City, addressOne.City);
        Assert.AreEqual(result.Country, addressOne.Country);
        Assert.AreEqual(result.PostalCode, addressOne.PostalCode);
      }

    }

    [TestMethod]
    public void AddCustomerOrderNewAddress()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var customer = TestHelper.CreateCustomer(resources.Context);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);

        var requestModel = new AddCustomerOrderAddressRequestModel
        {
          AddressType = "Delivery",
          AddressLine1 = "City Of Cape Town",
          AddressLine2 = "Adderley Street",
          City = "Cape Town",
          PostalCode = "7800",
          Country = "South Africa",
          Username = resources.TestUsername,
          OrderId = orderHead.OrderId,
          CustomerId = customer.CustomerId
        };

        // When 
        var result = resources.Controller.AddCustomerOrderAddress(requestModel) as string;

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual("Success", result);

        Assert.IsTrue(customer.Addresses.Any(o => o.AddressLine1 == requestModel.AddressLine1));
        Assert.IsTrue(customer.Addresses.Any(o => o.AddressLine2 == requestModel.AddressLine2));
        Assert.IsTrue(customer.Addresses.Any(o => o.City == requestModel.City));
        Assert.IsTrue(customer.Addresses.Any(o => o.Country == requestModel.Country));
        Assert.IsTrue(customer.Addresses.Any(o => o.PostalCode == requestModel.PostalCode));
        Assert.IsTrue(customer.Addresses.Any(o => o.AddressType == requestModel.AddressType));
        Assert.IsTrue(customer.Addresses.Any(o => o.CreateUser == requestModel.Username));
        Assert.AreEqual(customer.Addresses[0].AddressDetailId, orderHead.AddressDetailId);
      }

    }

    [TestMethod]
    public void AddCustomerOrderExistingAddress()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var customer = TestHelper.CreateCustomer(resources.Context);
        var orderHead = TestHelper.CreateOrderHead(resources.Context);
        var address = TestHelper.CreateCustomerAddress(resources.Context, customer);

        var requestModel = new AddCustomerOrderAddressRequestModel
        {
          AddressDetailId = address.AddressDetailId,
          AddressType = "Delivery",
          AddressLine1 = "City Of Cape Town",
          AddressLine2 = "Adderley Street",
          City = "Cape Town",
          PostalCode = "7800",
          Country = "South Africa",
          Username = resources.TestUsername,
          OrderId = orderHead.OrderId,
          CustomerId = customer.CustomerId
        };

        // When 
        var result = resources.Controller.AddCustomerOrderAddress(requestModel) as string;

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual("Success", result);

        Assert.IsTrue(customer.Addresses.Any(o => o.AddressLine1 == requestModel.AddressLine1));
        Assert.IsTrue(customer.Addresses.Any(o => o.AddressLine2 == requestModel.AddressLine2));
        Assert.IsTrue(customer.Addresses.Any(o => o.City == requestModel.City));
        Assert.IsTrue(customer.Addresses.Any(o => o.Country == requestModel.Country));
        Assert.IsTrue(customer.Addresses.Any(o => o.PostalCode == requestModel.PostalCode));
        Assert.IsTrue(customer.Addresses.Any(o => o.AddressType == requestModel.AddressType));
        Assert.IsTrue(customer.Addresses.Any(o => o.CreateUser == requestModel.Username));
        Assert.AreEqual(customer.Addresses[0].AddressDetailId, orderHead.AddressDetailId);
      }

    }

    [TestMethod]
    public void RemoveCustomerOrderAddress()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var customer = TestHelper.CreateCustomer(resources.Context);
        var address = TestHelper.CreateCustomerAddress(resources.Context, customer);
        var orderHead = TestHelper.CreateOrderHead(resources.Context, addressDetailId: address.AddressDetailId);

        var requestModel = new RemoveCustomerAddressRequestModel
        {
          AddressDetailId = address.AddressDetailId,
          CustomerId = customer.CustomerId,
          Username = resources.TestUsername
        };

        // When 
        var result = resources.Controller.RemoveCustomerOrderAddress(requestModel) as string;

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual("Success", result);
        Assert.AreEqual(0, customer.Addresses.Count);
        Assert.AreEqual(0, orderHead.AddressDetailId);
      }

    }

    [TestMethod]
    public void AcceptOrder()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        var order = TestHelper.CreateOrderHead(resources.Context);
        var requestModel = new AcceptOrderRequestModel { OrderId = order.OrderId, Username = resources.TestUsername };

        // when
        var result = resources.Controller.AcceptOrder(requestModel);

        // then
        Assert.IsNotNull(result);
        Assert.AreEqual("Success", result);
        Assert.AreEqual("ACCEPTED", order.Status.ToUpper());
        Assert.AreEqual(resources.TestUsername, order.UpdateUser);
      }
    }

    [TestMethod]
    public void GetOrderQuote()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        var companyProfile = TestHelper.CreateCompany(resources.Context);
        var companyAddress = TestHelper.CreateCompanyAddress(resources.Context, companyProfile);
        var companyBankingDetails = TestHelper.CreateCompanyBankingDetail(resources.Context, companyProfile);
        var customer = TestHelper.CreateCustomer(resources.Context, companyProfileId: companyProfile.CompanyProfileId);
        var customerAddress = TestHelper.CreateCustomerAddress(resources.Context, customer);
        var contactPerson = TestHelper.CreateContactPerson(resources.Context, customer);
        var order = TestHelper.CreateOrderHead(resources.Context, customerId: customer.CustomerId, companyProfileId: companyProfile.CompanyProfileId, contactId: contactPerson.ContactId, addressDetailId: customerAddress.AddressDetailId);
        var orderDetails = TestHelper.CreateOrderDetail(resources.Context, order);

        var requestModel = new GetOrderQuoteRequestModel { OrderId = order.OrderId, CompanyProfileId = companyProfile.CompanyProfileId };

        // When
        var result = resources.Controller.GetOrderQuote(requestModel) as OrderQuotationViewModel;

        // Then
        Assert.IsNotNull(result);

        // assert Company details
        Assert.AreEqual(result.CompanyProfile.AddressLine1, companyAddress.AddressLine1);
        Assert.AreEqual(result.CompanyProfile.AddressLine2, companyAddress.AddressLine2);
        Assert.AreEqual(result.CompanyProfile.City, companyAddress.City);
        Assert.AreEqual(result.CompanyProfile.Country, companyAddress.Country);
        Assert.AreEqual(result.CompanyProfile.DisplayName, companyProfile.DisplayName);
        Assert.AreEqual(result.CompanyProfile.EmailAddress, companyProfile.EmailAddress);
        Assert.AreEqual(result.CompanyProfile.PostalCode, companyAddress.PostalCode);
        Assert.AreEqual(result.CompanyProfile.RegistrationNo, companyProfile.RegistrationNo);
        Assert.AreEqual(result.CompanyProfile.TaxReferenceNo, companyProfile.TaxReferenceNo);
        Assert.AreEqual(result.CompanyProfile.TelephoneNo, companyProfile.TelephoneNo);

        // assert Company details
        Assert.AreEqual(result.CustomerDetail.CustomerAccountNo, customer.AccountNo);
        Assert.AreEqual(result.CustomerDetail.CustomerDetails, customer.CustomerDetails);
        Assert.AreEqual(result.CustomerDetail.CustomerEmailAddress, customer.EmailAddress);
        Assert.AreEqual(result.CustomerDetail.CustomerName, customer.CustomerName);
        Assert.AreEqual(result.CustomerDetail.CustomerContactNo, customer.ContactNo);
        Assert.AreEqual(result.CustomerDetail.ContactName, contactPerson.ContactName);
        Assert.AreEqual(result.CustomerDetail.ContactNo, contactPerson.ContactNo);
        Assert.AreEqual(result.CustomerDetail.ContactEmailAddress, contactPerson.EmailAddress);

        // assert delivery address details
        Assert.AreEqual(result.DeliveryAddress.AddressLine1, customerAddress.AddressLine1);
        Assert.AreEqual(result.DeliveryAddress.AddressLine2, customerAddress.AddressLine2);
        Assert.AreEqual(result.DeliveryAddress.City, customerAddress.City);
        Assert.AreEqual(result.DeliveryAddress.Country, customerAddress.Country);
        Assert.AreEqual(result.DeliveryAddress.PostalCode, customerAddress.PostalCode);

        // assert order details and order totals
        Assert.AreEqual(result.OrderTotals.OrderNo, order.OrderNo);
        Assert.AreEqual(result.OrderTotals.Discount, order.DiscountTotal);
        Assert.AreEqual(result.OrderTotals.OrderId, order.OrderId);
        Assert.AreEqual(result.OrderTotals.SubTotal, order.SubTotal);
        Assert.AreEqual(result.OrderTotals.Total, order.OrderTotal);
        Assert.AreEqual(result.OrderTotals.VatTotal, order.VatTotal);

        Assert.AreEqual(result.OrderTotals.OrderLineDetails[0].Discount, orderDetails.Discount);
        Assert.AreEqual(result.OrderTotals.OrderLineDetails[0].ItemDescription, orderDetails.ItemDescription);
        Assert.AreEqual(result.OrderTotals.OrderLineDetails[0].Quantity, orderDetails.Quantity);
        Assert.AreEqual(result.OrderTotals.OrderLineDetails[0].UnitPrice, orderDetails.UnitPrice);
        Assert.AreEqual(result.OrderTotals.OrderLineDetails[0].LineTotal, orderDetails.LineTotal);
      }
    }
  }
}
