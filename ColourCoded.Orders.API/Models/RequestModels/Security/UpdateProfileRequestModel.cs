namespace ColourCoded.Orders.API.Models.RequestModels.Security
{
  public class UpdateProfileRequestModel
  {
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
  }
}
