﻿using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;
using SpyderByteAPI.Resources.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGamesAccessor gamesAccessor;
        private readonly IStringLookup<ModelResult> modelResources;

        public GamesController(IGamesAccessor gamesAccessor, IStringLookup<ModelResult> modelResources)
        {
            this.gamesAccessor = gamesAccessor;
            this.modelResources = modelResources;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            IDataResponse<IList<Game>?> response = await gamesAccessor.GetAllAsync();

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
        public async Task<IActionResult> GetGame(int id)
        {
            IDataResponse<Game?> response = await gamesAccessor.GetSingleAsync(id);

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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Game game)
        {
            IDataResponse<Game?> response = await gamesAccessor.PostAsync(game);

            if (response.Result == ModelResult.Created)
            {
                // 201
                return CreatedAtAction(nameof(GetGame), new { id = response?.Data?.Id }, response?.Data);
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

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Patch(int id, [FromBody] Game game)
        {
            IDataResponse<Game?> response = await gamesAccessor.PatchAsync(id, game);

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            IDataResponse<Game?> response = await gamesAccessor.DeleteAsync(id);

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
