using System;

namespace ColourCoded.Orders.API.Data.Entities.Security
{
  public class User
  {
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public int CompanyProfileId { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
  }
}
