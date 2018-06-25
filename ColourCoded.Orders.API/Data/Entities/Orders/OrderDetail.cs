using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class OrderDetail
  {
    public int OrderDetailId { get; set; }
    public int LineNo { get; set; }
    public bool Negate { get; set; }
    public int OrderId { get; set; }
    public string ItemDescription { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
  }
}
