namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class HomeOrdersModel
  {
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
    public string CreateDate { get; set; }
    public string Total { get; set; }
    public string Status { get; set; }
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string ContactId { get; set; }
    public string ContactName { get; set; }
    public string EmailAddress { get; set; }
  }
}
