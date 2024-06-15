using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpyderByteAPI.Models.Users;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteServices.Services.Users.Abstract;

namespace SpyderByteAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;
        private readonly IMapper mapper;
        private readonly IStringLookup<ModelResult> modelResources;

        public UsersController(IUsersService usersService, IMapper mapper, IStringLookup<ModelResult> modelResources)
        {
            this.usersService = usersService;
            this.mapper = mapper;
            this.modelResources = modelResources;
        }

        [HttpGet("{id}")]
        [Authorize(PolicyType.ReadUsers)]
        public async Task<IActionResult> Get(Guid id)
        {
            var response = await usersService.GetAsync(id);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Users.User>(response.Data);
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
        [Authorize(PolicyType.WriteUsers)]
        public async Task<IActionResult> Post(PostUser user)
        {
            var response = await usersService.PostAsync(mapper.Map<SpyderByteServices.Models.Users.PostUser>(user));

            if (response.Result == ModelResult.Created)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Users.User>(response.Data);
                return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
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
            var response = await usersService.PatchAsync(mapper.Map<SpyderByteServices.Models.Users.PatchUser>(user));

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<SpyderByteAPI.Models.Users.User>(response.Data);
                return Ok(data);
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
                var data = mapper.Map<SpyderByteAPI.Models.Users.User>(response.Data);
                return Ok(data);
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
