using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ColourCoded.Orders.API.Shared
{
  public interface ISecurityHelper
  {
    string SaltedHashAlgorithm(string password, string saltValue);
    bool ValidatePassword(string password, string passwordHash);
    string CreateRandomPassword(int length);
  }
  public class SecurityHelper : ISecurityHelper
  {
    protected IConfiguration Configuration;

    public SecurityHelper(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public string SaltedHashAlgorithm(string password, string saltValue)
    {
      // Generate the salt bytes
      var encryptor = new RNGCryptoServiceProvider();
      var salt = Convert.FromBase64String(saltValue);
      encryptor.GetBytes(salt);

      // Hash the password and encode the parameters
      var hash = PasswordBasedKeyDerivation(password, salt, Convert.ToInt32(Configuration["Security.HashLength"]));

      string passwordHash = Convert.ToInt32(Configuration["Security.KeyIteration"]) + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
      return passwordHash;
    }

    public bool ValidatePassword(string password, string passwordHash)
    {
      // Extract the parameters from the hash
      char[] delimiter = { ':' };
      var split = passwordHash.Split(delimiter);
      var iterations = Int32.Parse(split[0]);
      var salt = Convert.FromBase64String(split[1]);
      var hash = Convert.FromBase64String(split[2]);

      var testHash = PasswordBasedKeyDerivation(password, salt, hash.Length);

      return Convert.ToBase64String(hash) == Convert.ToBase64String(testHash);
    }

    public string CreateRandomPassword(int length)
    {
      const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
      var tempPassword = new StringBuilder();
      var randomGenerator = new Random();

      while (0 < length--)
      {
        tempPassword.Append(valid[randomGenerator.Next(valid.Length)]);
      }
      return tempPassword.ToString();
    }

    private byte[] PasswordBasedKeyDerivation(string password, byte[] salt, int outputBytes)
    {
      Rfc2898DeriveBytes encryptor = new Rfc2898DeriveBytes(password, salt);
      encryptor.IterationCount = Convert.ToInt32(Configuration["Security.KeyIteration"]);
      return encryptor.GetBytes(outputBytes);
    }

  }
}
