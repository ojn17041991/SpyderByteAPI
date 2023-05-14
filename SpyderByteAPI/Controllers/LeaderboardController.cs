using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardAccessor leaderboardAccessor;

        public LeaderboardController(ILeaderboardAccessor leaderboardAccessor)
        {
            this.leaderboardAccessor = leaderboardAccessor;
        }

        [HttpGet("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string key)
        {
            IDataResponse<string> response = await leaderboardAccessor.GetAsync(key);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("{key}/{name}/{score}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(string key, string name, int score)
        {
            IDataResponse<string> response = await leaderboardAccessor.PostAsync(key, name, score);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok();
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
