using System;
using System.Collections.Generic;

namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class OrderDetailModel
  {
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime CreateDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal Total { get; set; }
    public decimal Discount { get; set; }
    public List<OrderLineDetailModel> OrderLineDetails { get; set; }
  }
}
