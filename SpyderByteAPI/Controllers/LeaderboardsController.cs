using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Services.Leaderboards.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly ILeaderboardsService leaderboardsService;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public LeaderboardsController(ILeaderboardsService leaderboardsService, IMapper mapper, IConfiguration configuration)
        {
            this.leaderboardsService = leaderboardsService;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            var response = await leaderboardsService.GetAsync(id);

            if (response.Result == ModelResult.OK)
            {
                // OJN: This hasn't been tested yet...
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.Leaderboard>(response.Data);
                return Ok(data);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Authorize(PolicyType.WriteLeaderboards)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] PostLeaderboard leaderboard)
        {
            var response = await leaderboardsService.PostAsync(mapper.Map<SpyderByteServices.Models.Leaderboards.PostLeaderboard>(leaderboard));

            if (response.Result == ModelResult.Created)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.Leaderboard>(response.Data);
                return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("Records")]
        [Authorize]
        [Authorize(PolicyType.WriteLeaderboards)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] PostLeaderboardRecord leaderboardRecord)
        {
            var response = await leaderboardsService.PostRecordAsync(mapper.Map<SpyderByteServices.Models.Leaderboards.PostLeaderboardRecord>(leaderboardRecord));

            if (response.Result == ModelResult.Created)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.LeaderboardRecord>(response.Data);
                return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("Records/{id}")]
        [Authorize]
        [Authorize(PolicyType.DeleteLeaderboards)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await leaderboardsService.DeleteRecordAsync(id);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.LeaderboardRecord>(response.Data);
                return Ok(data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
