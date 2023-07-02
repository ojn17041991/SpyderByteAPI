using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Resources.Abstract;
using SpyderByteAPI.Models.Jams;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JamsController : ControllerBase
    {
        private readonly IJamsAccessor jamsAccessor;
        private readonly IStringLookup<ModelResult> modelResources;
        private readonly IConfiguration configuration;

        public JamsController(IJamsAccessor jamsAccessor, IStringLookup<ModelResult> modelResources, IConfiguration configuration)
        {
            this.jamsAccessor = jamsAccessor;
            this.modelResources = modelResources;
            this.configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            IDataResponse<IList<Jam>?> response = await jamsAccessor.GetAllAsync();

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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJam(int id)
        {
            IDataResponse<Jam?> response = await jamsAccessor.GetSingleAsync(id);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                // 404
                return NotFound();
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] PostJam jam, [FromHeader] string sbApiKey)
        {
            if (configuration["SBAPIKEY"] != sbApiKey)
            {
                // 401
                return Unauthorized();
            }

            IDataResponse<Jam?> response = await jamsAccessor.PostAsync(jam);

            if (response.Result == ModelResult.Created)
            {
                // 201
                return CreatedAtAction(nameof(GetJam), new { id = response?.Data?.Id }, response?.Data);
            }
            else if (response.Result == ModelResult.AlreadyExists)
            {
                // 400
                return BadRequest(modelResources.GetResource(ModelResult.AlreadyExists));
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch([FromBody] PatchJam jam, [FromHeader] string sbApiKey)
        {
            if (configuration["SBAPIKEY"] != sbApiKey)
            {
                // 401
                return Unauthorized();
            }

            IDataResponse<Jam?> response = await jamsAccessor.PatchAsync(jam);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                // 404
                return NotFound();
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, [FromHeader] string sbApiKey)
        {
            if (configuration["SBAPIKEY"] != sbApiKey)
            {
                // 401
                return Unauthorized();
            }

            IDataResponse<Jam?> response = await jamsAccessor.DeleteAsync(id);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.NotFound)
            {
                // 404
                return NotFound();
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
