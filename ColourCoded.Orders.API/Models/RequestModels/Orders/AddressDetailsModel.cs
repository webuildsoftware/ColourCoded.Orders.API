using System;

namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class AddressDetailsModel
  {
    public string CustomerName { get; set; }
    public int AddressDetailId { get; set; }
    public string AddressType { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
