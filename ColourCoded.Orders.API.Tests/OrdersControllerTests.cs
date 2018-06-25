using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using ColourCoded.Orders.API.Shared;
using ColourCoded.Orders.API.Models.RequestModels.Orders;
using ColourCoded.Orders.API.Controllers;
using ColourCoded.Orders.API.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

      public Resources()
      {
        TestUsername = "testuser";
        Configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

        MemoryCache = new MemoryCache(new MemoryCacheOptions());
        Context = TestHelper.CreateDbContext(Configuration);
        Controller = new OrdersController(Context);
      }

    }

    [TestMethod]
    public void GetUserOrders_HomeOrdersModelList()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        const string username = "testuser";

        var customer = TestHelper.CreateCustomer(resources.Context, 1);
        var orderOne = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST001", salesPerson: username, salesDateAddMonths: -1);
        var orderTwo = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST002", salesPerson: username, salesDateAddMonths: -2);
        var orderThree = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST003", salesPerson: username, salesDateAddMonths: -3);

        var requestModel = new FindUserOrdersRequestModel { Username = username };

        // When
        var result = resources.Controller.GetUserOrders(requestModel);

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
    public void GetUserOrdersByPeriod_HomeOrdersModelList()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // Given
        const string username = "testuser";

        var customer = TestHelper.CreateCustomer(resources.Context, 1);
        var orderOne = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST001", salesPerson: username, salesDateAddMonths: -1);
        var orderTwo = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST002", salesPerson: username, salesDateAddMonths: -15);
        var orderThree = TestHelper.CreateOrderHead(resources.Context, orderNo: "TEST003", salesPerson: username, salesDateAddMonths: -30);

        var requestModel = new FindUserOrdersPeriodRequestModel { Username = username, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now };

        // When
        var result = resources.Controller.GetUserOrdersInPeriod(requestModel);

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
    public void AddOrderHead_Success_ReturnsInt()
    {
      var resources = new Resources();

      using (resources.Context.Database.BeginTransaction())
      {
        // given
        const string orderNo = "TEST123";
        var requestModel = new AddOrderRequestModel { OrderNo = orderNo, Username = resources.TestUsername};

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
        Assert.IsNull(savedOrderHead.UpdateDate);
        Assert.IsNull(savedOrderHead.UpdateUser);
        Assert.IsNotNull(savedOrderHead.OrderId);
        Assert.IsNotNull(savedOrderHead.CreateDate);
      }
    }

    [TestMethod]
    public void AddOrderHea_AlreadyExists_ReturnsInt()
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

  }
}
