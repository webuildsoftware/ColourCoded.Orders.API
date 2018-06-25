﻿using Microsoft.AspNetCore.Mvc;
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

    [HttpPost, Route("user")]
    public List<HomeOrdersModel> GetUserOrders([FromBody]FindUserOrdersRequestModel requestModel)
    {
      var noOrders = new List<HomeOrdersModel>();

      var userOrders = Context.Orders.Where(o => o.CreateUser == requestModel.Username).OrderByDescending(o => o.CreateDate)
        .Take(100).Select(o => new HomeOrdersModel
        {
          OrderId = o.OrderId,
          OrderNo = o.OrderNo,
          CreateDate = o.CreateDate.ToShortDateString(),
          Total = o.OrderTotal.ToString("R # ###.#0")
        }).ToList();

      return userOrders ?? noOrders;
    }

    [HttpPost, Route("userperiod")]
    public List<HomeOrdersModel> GetUserOrdersInPeriod([FromBody]FindUserOrdersPeriodRequestModel requestModel)
    {
      var noOrders = new List<HomeOrdersModel>();

      var userOrders = Context.Orders.Where(o => o.CreateUser == requestModel.Username && o.CreateDate.Date >= requestModel.StartDate && o.CreateDate.Date <= requestModel.EndDate)
        .Select(o => new HomeOrdersModel
        {
          OrderId = o.OrderId,
          OrderNo = o.OrderNo,
          CreateDate = o.CreateDate.ToShortDateString(),
          Total = o.OrderTotal.ToString("R 0 000.00")
        }).ToList();

      return userOrders ?? noOrders;
    }

    [HttpPost, Route("add")]
    public int AddOrder([FromBody]AddOrderRequestModel requestModel)
    {
      var existingOrder = Context.Orders.FirstOrDefault(o => o.OrderNo == requestModel.OrderNo);

      if (existingOrder != null)
        return existingOrder.OrderId;

      var newOrder = new OrderHead
      {
        OrderNo = requestModel.OrderNo,
        SubTotal = 0M,
        VatTotal = 0M,
        DiscountTotal = 0M,
        OrderTotal = 0M,
        VatRate = 0.15M, // to get from the context
        CreateDate = DateTime.Now,
        CreateUser = requestModel.Username
      };

      Context.Orders.Add(newOrder);
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
  }
}