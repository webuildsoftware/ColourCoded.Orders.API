using System;

namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class FindUserOrdersPeriodRequestModel
  {
    public string Username { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }
}
