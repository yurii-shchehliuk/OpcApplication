namespace QIA.Opc.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using QIA.Opc.Infrastructure.Application;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
    protected ActionResult HandleResponse<T>(ApiResponse<T> response)
    {
        if (response.IsSuccess)
        {
            return StatusCode((int)response.Status, response.Value);
        }

        return StatusCode((int)response.Status, response.Error);
    }
}
