namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class AddOrderDetailRequestModel
  {
    public int LineNo { get; set; }
    public int OrderId { get; set; }
    public string ItemDescription { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }
    public string Username { get; set; }
  }
}
