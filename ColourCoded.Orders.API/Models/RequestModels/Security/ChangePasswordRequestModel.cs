namespace ColourCoded.Orders.API.Models.RequestModels.Security
{
  public class ChangePasswordRequestModel
  {
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
  }
}
