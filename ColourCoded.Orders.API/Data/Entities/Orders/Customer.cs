using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class Customer
  {
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAccountNo { get; set; }
    public int? PersonId { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
  }
}

