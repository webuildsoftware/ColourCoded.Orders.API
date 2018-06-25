namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class OrderLineDetailModel
  {
    public int OrderId { get; set; }
    public string ItemDescription { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }
  }
}
