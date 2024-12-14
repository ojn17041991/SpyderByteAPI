using SpyderByteResources.Enums;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Games;

namespace SpyderByteServices.Services.Games.Abstract
{
    public interface IGamesService
    {
        Task<IDataResponse<IPagedList<Game>?>> GetAllAsync(string? name, GameType? type, int page, int pageSize, string? order, string? direction);

        Task<IDataResponse<Game?>> GetSingleAsync(Guid id);

        Task<IDataResponse<Game?>> PostAsync(PostGame game);

        Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame);

        Task<IDataResponse<Game?>> DeleteAsync(Guid id);
    }
}
