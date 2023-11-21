using MediatR;
using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.OPCUA.Connector.Entities;
using QIA.Opc.Domain.Request;
using QIA.Opc.Infrastructure.Services.OPCUA;

namespace QIA.Opc.API.Controllers
{
	public class SessionController : BaseController
	{
		private readonly SessionService sessionService;
		private readonly IMediator mediator;

		public SessionController(SessionService sessionService, IMediator mediator)
		{
			this.sessionService = sessionService;
			this.mediator = mediator;
		}

		[HttpPost("connect")]
		public async Task<ActionResult<SessionEntity>> ConnectToEndpoint([FromBody] SessionRequest request)
		{
			// reconnect
			var session = sessionService.GetSession(request.SessionId);
			if (session != null && request.Name == session.Name && session.State == Qia.Opc.Domain.Entities.Enums.SessionState.Connected)
			{
				return Ok(session);
			}

			// create new
			var newSession = await sessionService.CreateUniqueSessionAsync(request);
			if (newSession == null)
			{
				await mediator.Publish(new EventMediatorCommand(new Qia.Opc.Domain.Common.EventData
				{
					LogCategory = Qia.Opc.Domain.Entities.Enums.LogCategory.Error,
					Message = "Cannot connect to the provided URL",
					Title = "Session exception"
				}));
				return BadRequest(StatusCodes.Status418ImATeapot);
			}
			return Ok(newSession);
		}

		[HttpPost("create")]
		public async Task<ActionResult<SessionEntity>> CreateEndpoint([FromBody] SessionRequest request)
		{
			var session = await sessionService.CreateEndpointAsync(request);
			return Ok(session);
		}

		[HttpPost("disconnect")]
		public async Task<IActionResult> Disconnect([FromBody] SessionRequest request)
		{
			if (request.SessionId != null)
				await sessionService.Disconnect(request);

			return Ok();
		}

		[HttpPut("update")]
		public async Task<ActionResult<SessionEntity>> UpdateEndpoint([FromBody] SessionEntity request)
		{
			await sessionService.UpdateEndpointAsync(request);
			return Ok();
		}

		[HttpDelete("{sessionName}")]
		public async Task<IActionResult> DeleteSession(string sessionName)
		{
			await sessionService.DeleteSessionAsync(sessionName);
			return Ok();
		}

		[HttpGet("activeSessions")]
		public ActionResult<IEnumerable<SessionEntity>> ActiveSessions()
		{
			var sessionList = sessionService.GetActiveSessions();
			if (sessionList != null)
				return Ok(sessionList);
			else
				return NotFound();
		}

		[HttpGet("savedSessions")]
		public async Task<ActionResult<IEnumerable<SessionEntity>>> SavedSessions()
		{
			var sessionList = await sessionService.GetSavedSessionsAsync();
			if (sessionList != null)
				return Ok(sessionList);
			else
				return NotFound();
		}
	}
}
