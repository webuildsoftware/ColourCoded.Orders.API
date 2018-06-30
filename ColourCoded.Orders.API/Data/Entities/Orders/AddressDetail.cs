using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class AddressDetail
  {
    public int AddressDetailId { get; set; }
    public string AddressType { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public int CustomerId { get; set; }
    public string CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public string UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
  }
}
