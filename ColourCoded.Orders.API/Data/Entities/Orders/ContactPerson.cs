using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class ContactPerson
  {
    public int ContactId { get; set; }
    public string ContactName { get; set; }
    public string ContactNo { get; set; }
    public string EmailAddress { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
  }
}


