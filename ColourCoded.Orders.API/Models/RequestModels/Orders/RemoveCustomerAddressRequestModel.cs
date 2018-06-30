namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class RemoveCustomerAddressRequestModel
  {
    public int CustomerId { get; set; }
    public int AddressDetailId { get; set; }
    public string Username { get; set; }
  }
}

