using Newtonsoft.Json;
using QIA.Opc.Domain.Common;

namespace QIA.Opc.API.Core
{
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
				var user = context.User;
				await _next(context);
			}
			catch (Exception ex)
			{
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				var response = new
				{
					Status = HttpStatusCode.InternalServerError,
					Message = new string[]
					{
						ex.Message,
						ex.InnerException?.Message ?? "",
						ex.StackTrace ?? "",
					}
				};
				LoggerManager.Logger.Fatal(ex.Message + "{0}" + "{1}", ex.StackTrace, ex);

				var json = JsonConvert.SerializeObject(response);

				await context.Response.WriteAsync(json);
			}
		}
	}
}
