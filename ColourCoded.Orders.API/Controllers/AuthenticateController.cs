using ColourCoded.Orders.API.Data;
using ColourCoded.Orders.API.Data.Entities.Security;
using ColourCoded.Orders.API.Models.RequestModels.Security;
using ColourCoded.Orders.API.Models.ResponseModels;
using ColourCoded.Orders.API.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ColourCoded.Orders.API.Controllers
{
  [Route("api/authenticate")]
  [IgnoreOAth]
  public class AuthenticateController
  {
    protected SecurityContext Context;
    protected IMemoryCache MemoryCache;
    protected IConfiguration Configuration;
    protected ISecurityHelper SecurityHelper;
    protected ICommunicationsHelper CommunicationsHelper;
    public AuthenticateController(SecurityContext context, IMemoryCache memoryCache, IConfiguration configuration, ISecurityHelper securityHelper, ICommunicationsHelper communicationsHelper)
    {
      Context = context;
      MemoryCache = memoryCache;
      Configuration = configuration;
      SecurityHelper = securityHelper;
      CommunicationsHelper = communicationsHelper;
    }

    [HttpPost, Route("register")]
    public UserModel Register([FromBody]RegisterUserRequestModel requestModel)
    {
      if (requestModel.Password != requestModel.ConfirmPassword)
        throw new Exception("The passwords do not match.");

      var existingUser = Context.Users.FirstOrDefault(r => r.Username.ToUpper() == requestModel.Username.ToUpper());

      if (existingUser != null)
      {
        throw new Exception("The username already exists.");
      }

      var salt = Context.Salts.First(s => s.Active == true).Value;

      var user = new User
      {
        Username = requestModel.Username,
        Password = SecurityHelper.SaltedHashAlgorithm(requestModel.Password, salt), 
        FirstName = requestModel.FirstName,
        LastName = requestModel.LastName,
        EmailAddress = requestModel.EmailAddress,
        CreateDate = DateTime.Now,
        CreateUser = requestModel.Username
      };

      Context.Users.Add(user);
      Context.SaveChanges();

      var session = new Session
      {
        Username = user.Username,
        Token = Guid.NewGuid().ToString(),
        MethodName = "Register",
        Browser = requestModel.Browser,
        Device = requestModel.Device,
        CreateDate = DateTime.Now,
        ExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["Session.Expiration"]))
      };

      Context.Sessions.Add(session);
      Context.SaveChanges();

      MemoryCache.Set(session.Token, user.Username,
        new MemoryCacheEntryOptions
        {
          AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["Session.Expiration"]))),
          SlidingExpiration = new TimeSpan(0, Convert.ToInt32(Configuration["Session.Timeout"]), 0)
        });

      return new UserModel { Username = user.Username, ApiSessionToken = session.Token, IsAuthenticated = true };
      
    }

    [HttpPost, Route("login")]
    public UserModel Login([FromBody]LoginRequestModel requestModel)
    {
      var user = Context.Users.FirstOrDefault(r => r.Username.ToUpper() == requestModel.Username.ToUpper());
      
      if(user == null) // Username does not exist
        return new UserModel { Username = null };

      if(!SecurityHelper.ValidatePassword(requestModel.Password, user.Password)) // Incorrect Password
        return new UserModel { Username = user.Username, IsAuthenticated = false };

      // Authentication Successful
      var session = new Session
      {
        Username = user.Username,
        Token = Guid.NewGuid().ToString(),
        MethodName = "Login",
        Browser = requestModel.Browser,
        Device = requestModel.Device,
        CreateDate = DateTime.Now,
        ExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["Session.Expiration"]))
      };

      Context.Sessions.Add(session);
      Context.SaveChanges();

      MemoryCache.Set(session.Token, user.Username,
        new MemoryCacheEntryOptions
        {
          AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["Session.Expiration"]))),
          SlidingExpiration = new TimeSpan(0, Convert.ToInt32(Configuration["Session.Timeout"]), 0)
        });

      return new UserModel { Username = user.Username, ApiSessionToken = session.Token, IsAuthenticated = true, CompanyProfileId = user.CompanyProfileId };
    }

    [HttpPost, Route("profile/get")]
    public UpdateProfileModel GetProfile([FromBody]FindUserRequestModel requestModel)
    {
      var user = Context.Users.FirstOrDefault(r => r.Username.ToUpper() == requestModel.Username.ToUpper());

      if (user == null)
        return null;

      return new UpdateProfileModel
      {
        Username = user.Username,
        FirstName = user.FirstName,
        LastName = user.LastName,
        EmailAddress = user.EmailAddress
      }; 
    }

    [HttpPost, Route("profile/update")]
    public ValidationResult UpdateProfile([FromBody]UpdateProfileRequestModel requestModel)
    {
      var result = new ValidationResult();
      var user = Context.Users.FirstOrDefault(r => r.Username.ToUpper() == requestModel.Username.ToUpper());

      if (user == null)
      {
        result.InValidate("", "Username does not exist.");
        return result;
      }

      if(string.IsNullOrEmpty(requestModel.FirstName))
        result.InValidate("firstName", "First name required.");

      if (string.IsNullOrEmpty(requestModel.LastName))
        result.InValidate("lastName", "Last name required.");

      if (string.IsNullOrEmpty(requestModel.EmailAddress))
        result.InValidate("emailAddress", "Email address required.");

      if (!string.IsNullOrEmpty(requestModel.NewPassword) && requestModel.NewPassword.ToUpper() != requestModel.ConfirmPassword.ToUpper() )
        result.InValidate("password", "New and confirm password do not match.");

      if (result.IsValid)
      {
        user.UpdateDate = DateTime.Now;
        user.UpdateUser = user.Username;
        user.FirstName = requestModel.FirstName;
        user.LastName = requestModel.LastName;
        user.EmailAddress = requestModel.EmailAddress;

        if (!string.IsNullOrEmpty(requestModel.NewPassword))
        {
          var salt = Context.Salts.First(s => s.Active == true).Value;
          user.Password = SecurityHelper.SaltedHashAlgorithm(requestModel.NewPassword, salt);
        }

        Context.SaveChanges();
      }

      return result;
    }

    [HttpPost, Route("validateusername")]
    public ValidationResult FindUsername([FromBody]ValidateUserRequestModel requestModel)
    {
      var result = new ValidationResult();

      var existingUser = Context.Users.FirstOrDefault(r => r.Username.ToUpper() == requestModel.Username.ToUpper());

      if (existingUser == null)
      {
        return result;
      }
      else
      {
        result.InValidate("", "Username already exists");
        return result;
      }
    }

    [HttpPost, Route("validateuseremail")]
    public ValidationResult FindUserEmail([FromBody]ValidateEmailRequestModel requestModel)
    {
      var result = new ValidationResult();

      var existingUser = Context.Users.FirstOrDefault(r => r.EmailAddress.ToUpper() == requestModel.EmailAddress.ToUpper());

      if (existingUser == null)
      {
        return result;
      }
      else
      {
        result.InValidate("", "Already linked to another user.");
        return result;
      }
    }

    [HttpPost, Route("forgotpassword")]
    public ValidationResult ForgotPassword([FromBody]CredentialsRequestModel requestModel)
    {
      var result = new ValidationResult();

      var existingUser = Context.Users.FirstOrDefault(r => r.EmailAddress.ToUpper() == requestModel.EmailAddress.ToUpper());

      if (existingUser == null)
      {
        result.InValidate("", "The email address is not registered.");
        return result;
      }

      var tempPassword = SecurityHelper.CreateRandomPassword(Convert.ToInt32(Configuration["Security.TempPasswordLength"]));
      var salt = Context.Salts.First(s => s.Active == true).Value;
      var emailBody = @"Your temporary password is: " + tempPassword + Environment.NewLine +
                       "Please click on the following link to login: http://" + Configuration["ColourCoded.UI.Sitename"] + "/security/authenticate";

      existingUser.Password = SecurityHelper.SaltedHashAlgorithm(tempPassword, salt);
      Context.SaveChanges();

      if(CommunicationsHelper.SendEmail(existingUser.EmailAddress, "Password reset", emailBody))
        return result;

      result.InValidate("", "Error sending email. Please contact the IT Administrator.");
      return result;
    }

    [HttpPost, Route("changepassword")]
    public bool ChangePassword([FromBody]ChangePasswordRequestModel requestModel)
    {
      var user = Context.Users.FirstOrDefault(r => r.Username.ToUpper() == requestModel.Username.ToUpper());

      if (user == null)
        return false;

      // change password
      var salt = Context.Salts.First(s => s.Active == true).Value;

      user.Password = SecurityHelper.SaltedHashAlgorithm(requestModel.NewPassword, salt);
      user.UpdateDate = DateTime.Now;
      user.UpdateUser = user.Username;

      Context.SaveChanges();

      return true;
    }
  }
}
