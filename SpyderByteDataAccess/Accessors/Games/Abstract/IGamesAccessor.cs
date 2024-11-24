using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Games.Abstract
{
    public interface IGamesAccessor
    {
        Task<IDataResponse<IList<Game>?>> GetAllAsync(string? filter, int page, int count, string order, string direction);

        Task<IDataResponse<Game?>> GetSingleAsync(Guid id);

        Task<IDataResponse<Game?>> PostAsync(PostGame game);

        Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame);

        Task<IDataResponse<Game?>> DeleteAsync(Guid id);
    }
}
