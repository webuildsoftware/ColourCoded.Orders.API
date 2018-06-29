using System;
using ColourCoded.Orders.API.Data;
using ColourCoded.Orders.API.Data.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ColourCoded.Orders.API.Tests
{
  public static class TestHelper
  {
    public static OrdersContext CreateDbContext(IConfiguration configuration)
    {
      var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
      optionsBuilder.UseSqlServer(configuration.GetConnectionString("ColourCoded_Orders_OLTP"));

      return new OrdersContext(optionsBuilder.Options);
    }

    public static OrderHead CreateOrderHead(OrdersContext context, string orderNo = "TEST13993", string salesPerson = "testuser", decimal salesTotal = 1110.0M, decimal salesVat = 110.0M, int salesDateAddMonths = 0, int companyProfileId = 0, int customerId = 0, int contactId = 0)
    {
      var order = new OrderHead
      {
        OrderNo = orderNo,
        CreateDate = DateTime.Now.AddDays(salesDateAddMonths),
        CreateUser = salesPerson,
        OrderTotal = salesTotal,
        VatTotal = salesVat,
        SubTotal = salesTotal - salesVat,
        DiscountTotal = 0.0M,
        CompanyProfileId = companyProfileId,
        CustomerId = customerId,
        ContactId = contactId
      };

      context.Orders.Add(order);
      context.SaveChanges();

      return order;
    }

    public static OrderDetail CreateOrderDetail(OrdersContext context, OrderHead order)
    {
      var orderDetail = new OrderDetail
      {
        LineNo = 1,
        OrderId = order.OrderId,
        ItemDescription = "Test Product",
        UnitPrice = 100M,
        Quantity = 2,
        Discount = 0M,
        LineTotal = 200M,
        CreateDate = DateTime.Now,
        CreateUser = order.CreateUser
      };

      order.OrderDetails.Add(orderDetail);
      context.SaveChanges();

      return orderDetail;
    }

    public static Customer CreateCustomer(OrdersContext context, string customerName = "SMME", int companyProfileId = 0, string username = "sa")
    {
      var customer = new Customer
      {
        CustomerName = customerName,
        CompanyProfileId = companyProfileId,
        CustomerDetails = "test details",
        AccountNo = "DE232",
        ContactNo = "0213435566",
        EmailAddress = "someemail@gmail.com",
        MobileNo = "0823334444",
        CreateDate = DateTime.Now,
        CreateUser = username
      };

      context.Customers.Add(customer);
      context.SaveChanges();

      return customer;
    }

    public static ContactPerson CreateContactPerson(OrdersContext context, Customer customer, string contactName = "SMME", string username = "sa")
    {
      var newContact = new ContactPerson
      {
        ContactName = contactName,
        CustomerId = customer.CustomerId,
        ContactNo = "0214475588",
        EmailAddress = "contact@gmail.com",
        CreateDate = DateTime.Now,
        CreateUser = username
      };

      customer.ContactPeople.Add(newContact);
      context.SaveChanges();

      return newContact;
    }

    public static void RemoveTaxRates(OrdersContext context)
    {
      context.Database.ExecuteSqlCommand("truncate table TaxRates");
    }

    public static void RemoveOrderHeads(OrdersContext context)
    {
      context.Database.ExecuteSqlCommand("truncate table OrderHeads");
    }

    public static void RemoveCustomers(OrdersContext context)
    {
      context.Database.ExecuteSqlCommand("truncate table Customers");
      context.Database.ExecuteSqlCommand("truncate table ContactPersons");
    }

    public static TaxRate CreateTaxRate(OrdersContext context, decimal rate = 0.10M, string taxCode = "TESTTAX")
    {
      var taxRate = new TaxRate
      {
        Rate = rate,
        TaxCode = taxCode,
        StartDate = DateTime.Now.AddYears(-1),
        EndDate = DateTime.Now.AddYears(1),
        CreateDate = DateTime.Now,
        CreateUser = "sa"
      };

      context.TaxRates.Add(taxRate);
      context.SaveChanges();

      return taxRate;
    }

    public static CompanyProfile CreateCompany(OrdersContext context, int orderNoSeed = 1)
    {
      var company = new CompanyProfile
      {
        CreateDate = DateTime.Now,
        CreateUser = "sa",
        DisplayName = "Test Company",
        EmailAddress = "someemail@gmail.com",
        FaxNo = "0219999999",
        LegalName = "Test PTY LTD",
        OrderNoSeed = orderNoSeed,
        RegistrationNo = "TestReg34324",
        TaxReferenceNo = "VAT3e982/01",
        TelephoneNo = "0212923345"
      };

      context.CompanyProfiles.Add(company);
      context.SaveChanges();

      return company;
    }
  }
}
