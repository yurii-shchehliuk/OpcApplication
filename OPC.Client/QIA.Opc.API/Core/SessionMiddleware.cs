using Qia.Opc.OPCUA.Connector.Managers;

namespace QIA.Opc.API.Core
{
	public class SessionMiddleware
	{
		private readonly RequestDelegate _next;

		public SessionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, SessionManager sessionManager)
		{
			// Check if the request is targeting "SessionController"
			if (!(context.Request.Path.Value.Contains("/session") || context.Request.Path.Value.Contains("/swagger") ||
			context.Request.Path.Value.Contains("/chathub"))
			)
			{
				if (sessionManager.CurrentSession.State != Qia.Opc.Domain.Entities.Enums.SessionState.Connected)
				{
					context.Response.StatusCode = 401; // Unauthorized
					return;
				}
			}

			await _next(context);
		}
	}
}
