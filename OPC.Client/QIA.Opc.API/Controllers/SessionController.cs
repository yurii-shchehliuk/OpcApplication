namespace QIA.Opc.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using QIA.Opc.Application.Requests;
using QIA.Opc.Application.Responses;
using QIA.Opc.Infrastructure.Services.OPCUA;

public class SessionController : BaseController
{
    private readonly SessionService _sessionService;

    public SessionController(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<SessionResponse>> CreateEndpoint([FromBody] SessionRequest request)
    {

        Infrastructure.Application.ApiResponse<SessionResponse> sessionResponse = await _sessionService.CreateEndpointAsync(request);

        return HandleResponse(sessionResponse);

    }

    [HttpPost("connect")]
    public ActionResult<SessionResponse> ConnectToEndpoint([FromBody] SessionRequest request)
    {

        // reconnect
        Infrastructure.Application.ApiResponse<SessionResponse> currentSessionResponse = _sessionService.GetSessionIfActive(request.SessionNodeId);

        if (currentSessionResponse.Value != null)
        {
            return HandleResponse(currentSessionResponse);
        }

        // create new
        Infrastructure.Application.ApiResponse<SessionResponse> newSessionResponse = _sessionService.CreateUniqueSessionAsync(request);

        return HandleResponse(newSessionResponse);

     }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> SavedSessions()
    {

        Infrastructure.Application.ApiResponse<IEnumerable<SessionResponse>> sessionListResponse = await _sessionService.GetSessionsAsync();

        return HandleResponse(sessionListResponse);

    }

    [HttpPut("update")]
    public async Task<ActionResult<SessionResponse>> UpdateEndpoint([FromBody] SessionRequest request)
    {
        Infrastructure.Application.ApiResponse<Qia.Opc.Domain.Entities.SessionConfig> updatedSessionResponse = await _sessionService.UpdateEndpointAsync(request);

        return HandleResponse(updatedSessionResponse);

    }

    [HttpDelete("disconnect")]
    public IActionResult Disconnect([FromQuery] string sessionNodeId)
    {

        Infrastructure.Application.ApiResponse<bool> response = _sessionService.Disconnect(sessionNodeId);

        return HandleResponse(response);

    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteSession([FromQuery] string sessionGuid)
    {
        Infrastructure.Application.ApiResponse<bool> response = await _sessionService.DeleteSessionAsync(sessionGuid);

        return HandleResponse(response);

    }
}
