﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteServices.Services.Data.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DataController(IDataService dataService, IFeatureManager featureManager, IStringLookup<ModelResult> modelResources) : ControllerBase
    {
        private readonly IDataService dataService = dataService;
        private readonly IFeatureManager featureManager = featureManager;
        private readonly IStringLookup<ModelResult> modelResources = modelResources;

        [HttpPost("Backup")]
        [Authorize]
        [Authorize(PolicyType.DataBackup)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Backup()
        {
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowDatabaseBackups) == false)
            {
                return NotFound();
            }

            var response = await dataService.BackupAsync();

            if (response.Result == ModelResult.OK)
            {
                return Ok();
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

        [HttpDelete("Cleanup")]
        [Authorize]
        [Authorize(PolicyType.DataCleanup)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Cleanup()
        {
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowDatabaseCleanups) == false)
            {
                return NotFound();
            }

            var response = await dataService.CleanupAsync();

            if (response.Result == ModelResult.OK)
            {
                return Ok();
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
