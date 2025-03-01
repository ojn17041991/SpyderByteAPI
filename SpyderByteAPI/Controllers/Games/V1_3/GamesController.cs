using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteServices.Services.Games.Abstract;

namespace SpyderByteAPI.Controllers.V1_3
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.3")]
    [ApiController]
    public class GamesController(IGamesService gamesService, IMapper mapper) : ControllerBase
    {
        private readonly IGamesService gamesService = gamesService;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(
            [FromQuery] string? name,
            [FromQuery] GameType? type,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? order,
            [FromQuery] string? direction)
        {
            var response = await gamesService.GetAllAsync(name, type, page, pageSize, order, direction);

            if (response.Result == ModelResult.OK)
            {
                var data = mapper.Map<IPagedList<SpyderByteAPI.Models.Games.Game>>(response.Data);
                return Ok(data);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
