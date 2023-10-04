using Microsoft.AspNetCore.Mvc;
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
			var session = await sessionService.CreateUniqueSession(request);
			if (session == null)
			{
				return BadRequest(StatusCodes.Status400BadRequest);
			}
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
			await sessionService.DeleteSession(sessionName);
			return Ok();
		}

		[HttpGet("list")]
		public async Task<ActionResult<IEnumerable<SessionEntity>>> GetSessions()
		{
			var sessionList = await sessionService.GetSessionList();
			if (sessionList != null)
				return Ok(sessionList);
			else
				return NotFound();
		}
	}
}
