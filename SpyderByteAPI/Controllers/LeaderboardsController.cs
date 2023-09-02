﻿using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Leaderboard;

namespace SpyderByteAPI.Controllers
{
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly ILeaderboardAccessor leaderboardAccessor;
        private readonly IConfiguration configuration;

        public LeaderboardsController(ILeaderboardAccessor leaderboardAccessor, IConfiguration configuration)
        {
            this.leaderboardAccessor = leaderboardAccessor;
            this.configuration = configuration;
        }

        [HttpGet("[controller]/Games/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            IDataResponse<IList<LeaderboardRecord>?> response = await leaderboardAccessor.GetAsync(id);

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[controller]/Records")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] PostLeaderboardRecord leaderboardRecord)
        {
            IDataResponse<LeaderboardRecord?> response = await leaderboardAccessor.PostAsync(leaderboardRecord);

            if (response.Result == ModelResult.Created)
            {
                return CreatedAtAction(nameof(Get), new { id = response?.Data?.GameId }, response?.Data);
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

        [HttpDelete("[controller]/Records/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, [FromHeader] string sbApiKey)
        {
            if (configuration["SBAPIKEY"] != sbApiKey)
            {
                return Unauthorized();
            }

            IDataResponse<LeaderboardRecord?> response = await leaderboardAccessor.DeleteAsync(id);

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
    }
}