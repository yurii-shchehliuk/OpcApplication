using MediatR;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.OPCUA.Connector.Managers;
using static Azure.Core.HttpHeader;

namespace QIA.Opc.API.Core
{
	public class SessionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IMediator mediator;

		public SessionMiddleware(RequestDelegate next, IMediator mediator)
		{
			_next = next;
			this.mediator = mediator;
		}

		public async Task InvokeAsync(HttpContext context, SessionManager sessionManager)
		{
			if (ValidateSession(context.Request.Path.Value))
			{
				if (sessionManager.CurrentSession.State != Qia.Opc.Domain.Entities.Enums.SessionState.Connected)
				{
					context.Response.StatusCode = 401;
					return;
				}
			}

			await _next(context);
		}

		private bool ValidateSession(string pathValue)
		{
			var pathToValidate = new[] { "/node", "/subscription", "/tree" };

			return pathToValidate.Any(path => pathValue.ToLowerInvariant().Contains(path));
		}
	}
}
