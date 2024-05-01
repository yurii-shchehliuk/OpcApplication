namespace QIA.Opc.API.Middleware;

using Newtonsoft.Json;
using QIA.Opc.Infrastructure.Application;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            System.Security.Claims.ClaimsPrincipal user = context.User;
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            LoggerManager.Logger.Fatal("{0}", ex);

            var json = JsonConvert.SerializeObject(ex.Message);

            await context.Response.WriteAsync(json);
        }
    }
}
