using Microsoft.AspNetCore.Mvc;
using Qia.Opc.Domain.Core;

namespace QIA.Opc.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public abstract class BaseController : ControllerBase
	{
		protected ActionResult HandleResponse<T>(ApiResponse<T> response)
		{
			if (response == null)
			{
				return BadRequest();
			}

			if (response.IsSuccess)
			{
				return StatusCode((int)response.Status, response.Value);
			}
			else
			{
				return StatusCode((int)response.Status, response.Error);
			}
		}
	}
}
