using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.Models.Authentication;
using SpyderByteResources.Enums;
using SpyderByteServices.Services.Authentication.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AuthenticationController(IAuthenticationService authenticationService, IMapper mapper, IConfiguration configuration)
        {
            this.authenticationService = authenticationService;
            this.mapper = mapper;
            this.configuration = configuration;
        }

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
        public IActionResult Refresh()
        {
            if (!Convert.ToBoolean(configuration["AllowAuthenticationRefresh"] ?? "false"))
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
