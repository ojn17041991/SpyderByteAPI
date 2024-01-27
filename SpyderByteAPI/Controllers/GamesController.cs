using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authorization;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Resources.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGamesAccessor gamesAccessor;
        private readonly IStringLookup<ModelResult> modelResources;
        private readonly IConfiguration configuration;

        public GamesController(IGamesAccessor gamesAccessor, IStringLookup<ModelResult> modelResources, IConfiguration configuration)
        {
            this.gamesAccessor = gamesAccessor;
            this.modelResources = modelResources;
            this.configuration = configuration;
        }

        [HttpGet]
        [Authorize(PolicyType.ReadGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var response = await gamesAccessor.GetAllAsync();

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [Authorize(PolicyType.ReadGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var response = await gamesAccessor.GetSingleAsync(id);

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
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
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] PostGame game)
        {
            var response = await gamesAccessor.PostAsync(game);

            if (response.Result == ModelResult.Created)
            {
                return CreatedAtAction(nameof(GetGame), new { id = response?.Data?.Id }, response?.Data);
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
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch([FromForm] PatchGame game)
        {
            var response = await gamesAccessor.PatchAsync(game);

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
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
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await gamesAccessor.DeleteAsync(id);

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
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

        [HttpDelete]
        [Authorize(PolicyType.WriteGames)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClearRecords()
        {
            if (!Convert.ToBoolean(configuration["AllowDataClears"] ?? "false"))
            {
                return NotFound();
            }

            var response = await gamesAccessor.DeleteAllAsync();

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
