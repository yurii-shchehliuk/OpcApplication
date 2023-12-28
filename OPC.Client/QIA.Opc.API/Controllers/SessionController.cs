using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Core;
using Qia.Opc.Domain.Entities;
using QIA.Opc.Domain.Requests;
using QIA.Opc.Domain.Responses;
using QIA.Opc.Infrastructure.Services.OPCUA;

namespace QIA.Opc.API.Controllers
{
	public class SessionController : BaseController
	{
		private readonly SessionService sessionService;

		public SessionController(SessionService sessionService)
		{
			this.sessionService = sessionService;
		}

		[HttpGet("list")]
		public async Task<ActionResult<IEnumerable<SessionResponse>>> SavedSessions()
		{
			try
			{
				var sessionListResponse = await sessionService.GetSessionsAsync();

				return HandleResponse(sessionListResponse);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpPost("create")]
		public async Task<ActionResult<SessionResponse>> CreateEndpoint([FromBody] SessionRequest request)
		{
			try
			{
				var sessionResponse = await sessionService.CreateEndpointAsync(request);

				return HandleResponse(sessionResponse);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpPost("connect")]
		public ActionResult<SessionResponse> ConnectToEndpoint([FromBody] SessionRequest request)
		{
			try
			{
				// reconnect
				var currentSessionResponse = sessionService.GetSessionIfActive(request.SessionNodeId);

				if (currentSessionResponse.IsSuccess)
				{
					return HandleResponse(currentSessionResponse);
				}

				// create new
				var newSessionResponse = sessionService.CreateUniqueSessionAsync(request);

				return HandleResponse(newSessionResponse);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpDelete("disconnect")]
		public IActionResult Disconnect([FromQuery] string sessionNodeId)
		{
			try
			{
				var response = sessionService.Disconnect(sessionNodeId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpPut("update")]
		public async Task<ActionResult<SessionResponse>> UpdateEndpoint([FromBody] SessionRequest request)
		{
			try
			{
				var updatedSessionResponse = await sessionService.UpdateEndpointAsync(request);

				return HandleResponse(updatedSessionResponse);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteSession([FromQuery] string sessionNodeId)
		{
			try
			{
				var response = await sessionService.DeleteSessionAsync(sessionNodeId);

				return HandleResponse(response);
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Error("{0}", ex);
				throw;
			}
		}
	}
}
