using Microsoft.EntityFrameworkCore;
using ColourCoded.Orders.API.Data;
using ColourCoded.Orders.API.Data.Entities.Security;
using Microsoft.Extensions.Configuration;
using System;

namespace ColourCoded.Security.API.Tests
{
  public static class TestHelper
  {
    public static SecurityContext CreateDbContext(IConfiguration configuration)
    {
      var optionsBuilder = new DbContextOptionsBuilder<SecurityContext>();
      optionsBuilder.UseSqlServer(configuration.GetConnectionString("ColourCoded_Security_OLTP"));

      return new SecurityContext(optionsBuilder.Options);
    }

    public static void RemoveUsers(SecurityContext context)
    {
      context.Database.ExecuteSqlCommand("truncate table Users");
    }

    public static User CreateUser(SecurityContext context, string username = "testuser", string password = "password", string emailAddress = "test@gmail.com")
    {
      var user = new User
      {
        Username = username,
        Password = password,
        FirstName = "Test",
        LastName = "User",
        EmailAddress = emailAddress,
        CreateDate = DateTime.Now,
        CreateUser = "sa"
      };

      context.Users.Add(user);
      context.SaveChanges();

      return user;
    }
  }
}
