using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Qia.Opc.Domain.Core
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;
		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_env = env;
			_logger = logger;
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				var user = context.User;
				//context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
						ex.StackTrace,
						ex.InnerException?.Message ?? ""
					}
				};
				LoggerManager.Logger.Fatal(ex.Message + "{0}" + "{1}", ex.StackTrace, ex);

				var json = JsonConvert.SerializeObject(response);

				await context.Response.WriteAsync(json);
			}
		}
	}
}
