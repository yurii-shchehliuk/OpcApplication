using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.DTO;
using Qia.Opc.Domain.Entities;
using Qia.Opc.Infrastrucutre.Services.OPCUA;
using Qia.Opc.OPCUA.Connector.Entities;

namespace QIA.Opc.API.Controllers
{
	public class SessionController : BaseController
	{
		private readonly SessionService sessionService;

		public SessionController(SessionService sessionService)
		{
			this.sessionService = sessionService;
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
