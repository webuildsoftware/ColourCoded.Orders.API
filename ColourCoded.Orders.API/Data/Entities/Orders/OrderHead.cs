using System;
using System.Collections.Generic;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class OrderHead
  {
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal OrderTotal { get; set; }
    public decimal VatRate { get; set; }
    public int CompanyProfileId { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }

    public OrderHead()
    {
      OrderDetails = new List<OrderDetail>();
    }
  }
}
