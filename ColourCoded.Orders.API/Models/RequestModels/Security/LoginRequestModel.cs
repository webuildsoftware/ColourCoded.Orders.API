namespace ColourCoded.Orders.API.Models.RequestModels.Security
{
  public class LoginRequestModel
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string Browser { get; set; }
    public string Device { get; set; }
  }
}
