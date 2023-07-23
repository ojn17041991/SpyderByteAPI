using Microsoft.AspNetCore.Mvc;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WakeUpController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Post()
        {
            return NoContent();
        }
    }
}
