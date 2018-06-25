using System;

namespace ColourCoded.Orders.API.Data.Entities.Security
{
  public class Session
  {
    public int SessionId { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
    public string MethodName { get; set; }
    public string Browser { get; set; }
    public string Device { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ExpirationDate { get; set; }
  }
}
