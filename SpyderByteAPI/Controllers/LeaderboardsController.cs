using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.Models.Leaderboards;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteServices.Services.Leaderboards.Abstract;
using SpyderByteResources.Extensions;

namespace SpyderByteAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly ILeaderboardsService leaderboardsService;
        private readonly SpyderByteServices.Services.Authorization.Abstract.IAuthorizationService authorizationService;
        private readonly IMapper mapper;
        private readonly IStringLookup<ModelResult> modelResources;
        private readonly IConfiguration configuration;

        public LeaderboardsController(
            ILeaderboardsService leaderboardsService,
            SpyderByteServices.Services.Authorization.Abstract.IAuthorizationService authorizationService,
            IMapper mapper,
            IStringLookup<ModelResult> modelResources,
            IConfiguration configuration)
        {
            this.leaderboardsService = leaderboardsService;
            this.authorizationService = authorizationService;
            this.mapper = mapper;
            this.modelResources = modelResources;
            this.configuration = configuration;
        }

        [HttpGet("{id}")]
        [Authorize]
        [Authorize(PolicyType.ReadLeaderboards)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            var authorizationResponse = await authorizationService.UserHasAccessToLeaderboard(HttpContext.GetLoggedInUserId(), id);
            if (authorizationResponse.Result == ModelResult.Unauthorized)
            {
                return Unauthorized();
            }

            var response = await leaderboardsService.GetAsync(id);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.Leaderboard>(response.Data);
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

        [HttpPost]
        [Authorize]
        [Authorize(PolicyType.WriteLeaderboards)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostLeaderboard([FromBody] PostLeaderboard leaderboard)
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
            else if (response.Result == ModelResult.AlreadyExists)
            {
                return BadRequest(modelResources.GetResource(ModelResult.AlreadyExists));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("Records")]
        [Authorize]
        [Authorize(PolicyType.WriteLeaderboardRecords)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostRecord([FromBody] PostLeaderboardRecord leaderboardRecord)
        {
            var authorizationResponse = await authorizationService.UserHasAccessToLeaderboard(HttpContext.GetLoggedInUserId(), leaderboardRecord.LeaderboardId);
            if (authorizationResponse.Result == ModelResult.Unauthorized)
            {
                return Unauthorized();
            }

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

        [HttpPatch]
        [Authorize]
        [Authorize(PolicyType.WriteLeaderboards)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchLeaderboard([FromBody] PatchLeaderboard leaderboard)
        {
            var response = await leaderboardsService.PatchAsync(mapper.Map<SpyderByteServices.Models.Leaderboards.PatchLeaderboard>(leaderboard));

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.Leaderboard>(response.Data);
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

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(PolicyType.DeleteLeaderboards)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLeaderboard(Guid id)
        {
            var response = await leaderboardsService.DeleteAsync(id);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Leaderboards.Leaderboard>(response.Data);
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

        [HttpDelete("Records/{id}")]
        [Authorize]
        [Authorize(PolicyType.DeleteLeaderboardRecords)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRecord(Guid id)
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
