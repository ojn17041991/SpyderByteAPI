﻿using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] PostJam jam, [FromHeader] string sbApiKey)
        {
            if (configuration["SBAPIKEY"] != sbApiKey)
            {
                // 401
                return Unauthorized();
            }

            IDataResponse<Jam?> response = await jamsAccessor.PostAsync(jam);

            if (response.Result == ModelResult.OK)
            {
                // 200
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.AlreadyExists)
            {
                // 400
                return BadRequest(modelResources.GetResource(ModelResult.AlreadyExists));
            }
            else if (response.Result == ModelResult.Unauthorized)
            {
                // 401
                return Unauthorized();
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
        public async Task<IActionResult> Patch([FromForm] PatchJam jam, [FromHeader] string sbApiKey)
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

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClearRecords([FromHeader] string sbApiKey)
        {
            return NotFound();
            if (configuration["SBAPIKEY"] != sbApiKey)
            {
                // 401
                return Unauthorized();
            }

            var response = await jamsAccessor.DeleteAllAsync();

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
