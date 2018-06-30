namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class AddCustomerOrderAddressRequestModel
  {
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public int AddressDetailId { get; set; }
    public string AddressType { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string Username { get; set; }
  }
}
