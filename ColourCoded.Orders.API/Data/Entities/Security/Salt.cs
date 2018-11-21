using System;

namespace ColourCoded.Orders.API.Data.Entities.Security
{
  public class Salt
  {
    public int SaltId { get; set; }
    public string Value { get; set; }
    public bool Active { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
  }
}
