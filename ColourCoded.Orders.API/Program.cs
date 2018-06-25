using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ColourCoded.Orders.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var configuration = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json")
         .AddEnvironmentVariables()
         .Build();

      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

      try
      {
        Log.Information("Starting web host");
        BuildWebHost(args).Run();
        return;
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Web Host terminated unexpectedly");
        return;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseSerilog()
            .Build();
  }
}
