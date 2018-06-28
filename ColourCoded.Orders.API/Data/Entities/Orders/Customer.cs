using System;
using System.Collections.Generic;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class Customer
  {
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerDetails { get; set; }
    public string ContactNo { get; set; }
    public string AccountNo { get; set; }
    public string MobileNo { get; set; }
    public string EmailAddress { get; set; }
    public int CompanyProfileId { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
    public List<ContactPerson> ContactPeople { get; set; }

    public Customer()
    {
      ContactPeople = new List<ContactPerson>();
    }
  }
}


