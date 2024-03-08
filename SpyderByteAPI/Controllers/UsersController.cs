using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authorization;
using SpyderByteAPI.Models.Users;
using SpyderByteAPI.Resources.Abstract;
using SpyderByteAPI.Services.Users.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;
        private readonly IStringLookup<ModelResult> modelResources;

        public UsersController(IUsersService usersService, IStringLookup<ModelResult> modelResources)
        {
            this.usersService = usersService;
            this.modelResources = modelResources;
        }

        [HttpGet("{id}")]
        [Authorize(PolicyType.ReadUsers)]
        public async Task<IActionResult> Get(Guid id)
        {
            var response = await usersService.GetAsync(id);

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

        [HttpPost]
        [Authorize(PolicyType.WriteUsers)]
        public async Task<IActionResult> Post(PostUser user)
        {
            var response = await usersService.PostAsync(user);

            if (response.Result == ModelResult.Created)
            {
                return Created(string.Empty, response.Data);
            }
            else if (response.Result == ModelResult.AlreadyExists)
            {
                return BadRequest(modelResources.GetResource(ModelResult.AlreadyExists));
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return BadRequest(modelResources.GetResource(ModelResult.NotFound));
            }
            else if (response.Result == ModelResult.RequestInvalid)
            {
                return BadRequest(modelResources.GetResource(ModelResult.RequestInvalid));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch]
        [Authorize(PolicyType.WriteUsers)]
        public async Task<IActionResult> Patch(PatchUser user)
        {
            var response = await usersService.PatchAsync(user);

            if (response.Result == ModelResult.OK)
            {
                return Ok();
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound();
            }
            else if (response.Result == ModelResult.RequestInvalid)
            {
                return BadRequest(modelResources.GetResource(ModelResult.RequestInvalid));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(PolicyType.WriteUsers)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await usersService.DeleteAsync(id);

            if (response.Result == ModelResult.OK)
            {
                return NoContent();
            }
            else if (response.Result == ModelResult.NotFound)
            {
                return NotFound();
            }
            else if (response.Result == ModelResult.RequestInvalid)
            {
                return BadRequest(modelResources.GetResource(ModelResult.RequestInvalid));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
