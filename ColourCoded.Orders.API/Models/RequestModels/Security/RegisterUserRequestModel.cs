namespace ColourCoded.Orders.API.Models.RequestModels.Security
{
  public class RegisterUserRequestModel
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Browser { get; set; }
    public string Device { get; set; }
  }
}
