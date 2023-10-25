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
