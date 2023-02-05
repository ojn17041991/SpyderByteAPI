using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;
using SpyderByteAPI.Models.Abstract;
using SpyderByteAPI.Resources.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly BaseGamesAccessor<Game> gameAccessor;
        private readonly IStringLookup<ModelResult> modelResources;

        public GamesController(BaseGamesAccessor<Game> gameAccessor, IStringLookup<ModelResult> modelResources)
        {
            this.gameAccessor = gameAccessor;
            this.modelResources = modelResources;
        }



        // GET: api/<GamesController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            IDataResponse<IQueryable<IGame>> response = gameAccessor.Get();

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

        // GET api/<GamesController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGame(int id)
        {
            IDataResponse<IGame?> response = gameAccessor.Get(id);

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

        // POST api/<GamesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] Game game)
        {
            IDataResponse<IGame?> response = gameAccessor.Post(game);

            if (response.Result == ModelResult.Created)
            {
                // 201
                return CreatedAtAction(nameof(GetGame), new { id = response?.Data?.Id }, response?.Data);
            }
            else if (response.Result == ModelResult.IDGivenForIdentityField)
            {
                // 400
                return BadRequest(modelResources.GetResource(ModelResult.IDGivenForIdentityField));
            }
            else
            {
                // 500
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT api/<GamesController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put(int id, [FromBody] Game game)
        {
            IDataResponse<IGame?> response = gameAccessor.Put(id, game);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.IDMismatchInPut)
            {
                // 400
                return BadRequest(modelResources.GetResource(ModelResult.IDMismatchInPut));
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

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Patch(int id, [FromBody] Game game)
        {
            IDataResponse<IGame?> response = gameAccessor.Patch(id, game);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.IDFoundInPatch)
            {
                // 400
                return BadRequest(modelResources.GetResource(ModelResult.IDFoundInPatch));
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

        // DELETE api/<GamesController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            IDataResponse<IGame?> response = gameAccessor.Delete(id);

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
