using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Get()
        //{
        //    IDataResponse<IList<Game>?> response = await gamesAccessor.GetAllAsync();

        //    if (response.Result == ModelResult.OK)
        //    {
        //        // 200
        //        return Ok(response.Data);
        //    }
        //    else
        //    {
        //        // 500
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
    }
}
