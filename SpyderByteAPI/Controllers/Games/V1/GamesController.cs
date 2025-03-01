using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteServices.Services.Games.Abstract;

namespace SpyderByteAPI.Controllers.Games.V1
{
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public class GamesController(IGamesService gamesService, IFeatureManager featureManager, IMapper mapper, IStringLookup<ModelResult> modelResources) : ControllerBase
    {
        private readonly IGamesService gamesService = gamesService;
        private readonly IFeatureManager featureManager = featureManager;
        private readonly IMapper mapper = mapper;
        private readonly IStringLookup<ModelResult> modelResources = modelResources;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowUseOfNonPaginatedEndpoints) == false)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, modelResources.GetResource(ModelResult.NotImplemented));
            }
            
            var response = await gamesService.GetAllAsync(null!, null!, 1, Int32.MaxValue, null!, null!);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<IList<Game>>(response.Data!.Items);
                return Ok(data);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var response = await gamesService.GetSingleAsync(id);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<Game>(response.Data);
                return Ok(data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound(modelResources.GetResource(ModelResult.NotFound));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] PostGame game)
        {
            var response = await gamesService.PostAsync(mapper.Map<SpyderByteServices.Models.Games.PostGame>(game));

            if (response.Result == ModelResult.Created)
            {
                var data = mapper.Map<Game>(response.Data);
                return CreatedAtAction(nameof(GetGame), new { id = data.Id }, data);
            }
            else if (response.Result == ModelResult.AlreadyExists)
            {
                return BadRequest(modelResources.GetResource(ModelResult.AlreadyExists));
            }
            else if (response.Result == ModelResult.RequestDataIncomplete)
            {
                return BadRequest(modelResources.GetResource(ModelResult.RequestDataIncomplete));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch]
        [Authorize]
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch([FromForm] PatchGame game)
        {
            var response = await gamesService.PatchAsync(mapper.Map<SpyderByteServices.Models.Games.PatchGame>(game));

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<Game>(response.Data);
                return Ok(data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound(modelResources.GetResource(ModelResult.NotFound));
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

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await gamesService.DeleteAsync(id);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<Game>(response.Data);
                return Ok(data);
            }
            else if (response.Result == ModelResult.RelationshipViolation)
            {
                return BadRequest(modelResources.GetResource(ModelResult.RelationshipViolation));
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound(modelResources.GetResource(ModelResult.NotFound));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
