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

    public static SecurityContext CreateSecurityContext(IConfiguration configuration)
    {
      var optionsBuilder = new DbContextOptionsBuilder<SecurityContext>();
      optionsBuilder.UseSqlServer(configuration.GetConnectionString("ColourCoded_Security_OLTP"));

      return new SecurityContext(optionsBuilder.Options);
    }

    public static OrderHead CreateOrderHead(OrdersContext context, string orderNo = "TEST13993", string salesPerson = "testuser", decimal salesTotal = 1110.0M, decimal salesVat = 110.0M, int salesDateAddMonths = 0, int companyProfileId = 0, int customerId = 0, int contactId = 0, int addressDetailId = 0)
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
        ContactId = contactId,
        AddressDetailId = addressDetailId
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

    public static AddressDetail CreateCustomerAddress(OrdersContext context, Customer customer, string addressType = "Delivery", string addressLine1 = "SMME", string addressLine2 = "sa", string city = "Cape Town", string country = "South Africa", string username = "testuser")
    {
      var newAddress = new AddressDetail
      {
        AddressType = addressType,
        AddressLine1 = addressLine1,
        AddressLine2 = addressLine2,
        City = city,
        PostalCode = "7786",
        Country = country,
        CreateUser = username,
        CreateDate = DateTime.Now
      };

      customer.Addresses.Add(newAddress);
      context.SaveChanges();

      return newAddress;
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

    public static CompanyAddressDetail CreateCompanyAddress(OrdersContext context, CompanyProfile company, string addressType = "Delivery", string addressLine1 = "SMME", string addressLine2 = "sa", string city = "Cape Town", string country = "South Africa", string username = "testuser")
    {
      var newAddress = new CompanyAddressDetail
      {
        AddressType = addressType,
        AddressLine1 = addressLine1,
        AddressLine2 = addressLine2,
        City = city,
        PostalCode = "7786",
        Country = country,
        CreateUser = username,
        CreateDate = DateTime.Now
      };

      company.Addresses.Add(newAddress);
      context.SaveChanges();

      return newAddress;
    }

    public static CompanyBankingDetail CreateCompanyBankingDetail(OrdersContext context, CompanyProfile company, string accountType = "Savings", string bankName = "ABSA", string branchCode = "632005", string accountNo = "90000332", string accountHolder = "Mr Absa Banker", string username = "testuser")
    {
      var newBankingDetail = new CompanyBankingDetail
      {
        AccountType = accountType,
        AccountHolder = accountHolder,
        AccountNo = accountNo,
        BankName = bankName,
        BranchCode = branchCode,
        CreateUser = username,
        CreateDate = DateTime.Now
      };

      company.BankingDetails.Add(newBankingDetail);
      context.SaveChanges();

      return newBankingDetail;
    }

  }
}
