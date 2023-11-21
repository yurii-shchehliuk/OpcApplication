using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qia.Opc.Domain.Core;
using Qia.Opc.Infrastructure.Application;
using System.Net;

namespace QIA.Opc.API.Core
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IHostEnvironment _env;
		private readonly IMediator mediator;

		public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env, IMediator mediator)
		{
			_env = env;
			this.mediator = mediator;
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
						ex.InnerException?.Message ?? "",
						ex.StackTrace ?? "",
					}
				};
				LoggerManager.Logger.Fatal(ex.Message + "{0}" + "{1}", ex.StackTrace, ex);

				var json = JsonConvert.SerializeObject(response);

				await context.Response.WriteAsync(json);

				await mediator.Publish(new EventMediatorCommand(new Qia.Opc.Domain.Common.EventData()
				{
					LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Error,
					Message = ex.Message,
					Title = "Exception occured"
				}));
			}
		}
	}
}
