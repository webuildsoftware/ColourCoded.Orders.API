using System.Collections.Generic;

namespace ColourCoded.Orders.API.Shared
{
  public class ValidationResult
  {
    public bool IsValid { get; set; }
    public List<ValidationResultMessage> Messages { get; set; }

    public ValidationResult()
    {
      Messages = new List<ValidationResultMessage>();
      IsValid = true;
    }

    public void InValidate(string messageCode, string messageText)
    {
      IsValid = false;
      Messages.Add(new ValidationResultMessage { MessageCode = messageCode, Message = messageText });
    }
  }
}
