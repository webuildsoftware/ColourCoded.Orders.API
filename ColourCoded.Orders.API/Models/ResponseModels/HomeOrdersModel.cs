namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class HomeOrdersModel
  {
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
    public string CreateDate { get; set; }
    public string Total { get; set; }
    public string Status { get; set; }
  }
}
