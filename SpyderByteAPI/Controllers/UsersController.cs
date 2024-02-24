using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authorization;
using SpyderByteAPI.Models.Users;
using SpyderByteAPI.Resources.Abstract;
using SpyderByteAPI.Services.Users.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [Authorize(PolicyType.WriteUsers)]
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

        [HttpPost]
        public async Task<IActionResult> Post(PostUser user)
        {
            var response = await usersService.PostAsync(user);

            if (response.Result == ModelResult.Created)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            else if (response.Result == ModelResult.AlreadyExists)
            {
                return BadRequest(modelResources.GetResource(ModelResult.AlreadyExists));
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
        public async Task<IActionResult> Delete(string id)
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
