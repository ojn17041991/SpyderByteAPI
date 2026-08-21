using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SpyderByteAPI.Controllers.Health.V1_4
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.4")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }
    }
}
