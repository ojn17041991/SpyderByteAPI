using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SpyderByteAPI.Models.Authentication;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteServices.Services.Authentication.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService, IMapper mapper, IFeatureManager featureManager) : ControllerBase
    {
        private readonly IAuthenticationService authenticationService = authenticationService;
        private readonly IMapper mapper = mapper;
        private readonly IFeatureManager featureManager = featureManager;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(Login login)
        {
            var response = await authenticationService.AuthenticateAsync(mapper.Map<SpyderByteServices.Models.Authentication.Login>(login));

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.Unauthorized)
            {
                return Unauthorized();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("Refresh")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh()
        {
            if (await featureManager.IsEnabledAsync(FeatureFlags.AllowAuthenticationRefresh) == false)
            {
                return NotFound();
            }

            var response = authenticationService.Refresh(HttpContext);

            if (response.Result == ModelResult.OK)
            {
                return Ok(response.Data);
            }
            else if (response.Result == ModelResult.Unauthorized)
            {
                return Unauthorized();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete()
        {
            var response = authenticationService.Deauthenticate(HttpContext);

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
