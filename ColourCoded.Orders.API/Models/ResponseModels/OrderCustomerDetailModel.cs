using System;

namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class OrderCustomerDetailModel
  {
    public int CustomerId { get; set; }
    public int ContactId { get; set; }
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime OrderCreateDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerDetails { get; set; }
    public string CustomerContactNo { get; set; }
    public string CustomerAccountNo { get; set; }
    public string CustomerMobileNo { get; set; }
    public string CustomerEmailAddress { get; set; }
    public string ContactName { get; set; }
    public string ContactNo { get; set; }
    public string ContactEmailAddress { get; set; }
    public bool ContactAdded { get; set; }
  }
}
