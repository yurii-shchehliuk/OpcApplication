using MediatR;
using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.Infrastrucutre.Services.OPCUA;
using Qia.Opc.OPCUA.Connector.Entities;

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
		public async Task<ActionResult<SessionEntity>> ConnectToEndpoint([FromBody] SessionDTO request)
		{
			// reconnect
			var session = sessionService.GetSession(request.SessionId);
			if (session != null && request.Name == session.Name)
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
				return BadRequest(StatusCodes.Status400BadRequest);
			}
			return Ok(newSession);
		}

		[HttpPost("create")]
		public async Task<ActionResult<SessionEntity>> CreateEndpoint([FromBody] SessionDTO request)
		{
			var session = await sessionService.CreateEndpointAsync(request);
			return Ok(session);
		}

		[HttpPut("update")]
		public async Task<ActionResult<SessionEntity>> UpdateEndpoint([FromBody] SessionEntity request)
		{
			await sessionService.UpdateEndpointAsync(request);
			return Ok();
		}

		[HttpGet("{sessionId}")]
		public ActionResult<SessionEntity> GetSession(string sessionId)
		{
			var session = sessionService.GetSession(sessionId);
			if (session != null)
				return Ok(session);
			else
				return NotFound();
		}

		[HttpDelete("{sessionName}")]
		public async Task<IActionResult> DeleteSession(string sessionName)
		{
			await sessionService.DeleteSessionAsync(sessionName);
			return Ok();
		}

		[HttpGet("list")]
		public async Task<ActionResult<IEnumerable<SessionEntity>>> GetSessions()
		{
			var sessionList = await sessionService.GetSessionListAsync();
			if (sessionList != null)
				return Ok(sessionList);
			else
				return NotFound();
		}
	}
}
