using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteResources.Helpers.FeatureFlags;
using SpyderByteServices.Services.Data.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService dataService;
        private readonly IFeatureManager featureManager;

        public DataController(IDataService dataService, IFeatureManager featureManager)
        {
            this.dataService = dataService;
            this.featureManager = featureManager;
        }

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

            var response = await dataService.Backup();

            if (response.Result == ModelResult.OK)
            {
                return Ok();
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
