using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ColourCoded.Orders.API.Shared
{
  public class GlobalExceptionFilter : ExceptionFilterAttribute
  {
    public override void OnException(ExceptionContext context)
    {
      // handle logging here
      Log.Error(context.Exception.Message);
      Log.Error(context.Exception.StackTrace);

      context.Result = new JsonResult(context.Exception);
      context.HttpContext.Response.StatusCode = 500;

      base.OnException(context);
    }
  }
}
