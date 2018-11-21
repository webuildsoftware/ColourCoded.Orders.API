using Microsoft.Extensions.Configuration;

namespace ColourCoded.Orders.API.Shared
{
  public interface ICommunicationsHelper
  {
    bool SendEmail(string emailAddress, string subject, string bodyText);
  }
  public class CommunicationsHelper : ICommunicationsHelper
  {
    protected IConfiguration Configuration;

    public CommunicationsHelper(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public bool SendEmail(string emailAddress, string subject, string bodyText)
    {
      return true;
    }
  }
}
