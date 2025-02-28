using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Games.Abstract
{
    public interface IGamesAccessor
    {
        Task<IDataResponse<IPagedList<Game>?>> GetAllAsync(string? name, GameType? type, int page, int pageSize, string? order, string? direction);

        Task<IDataResponse<Game?>> GetSingleAsync(Guid id);

        Task<IDataResponse<Game?>> GetSingleByNameAsync(string name);

        Task<IDataResponse<Game?>> PostAsync(PostGame game);

        Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame);

        Task<IDataResponse<Game?>> DeleteAsync(Guid id);
    }
}
