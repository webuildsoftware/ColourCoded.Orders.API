namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class GetCustomerOrderAddressRequestModel
  {
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
  }
}
