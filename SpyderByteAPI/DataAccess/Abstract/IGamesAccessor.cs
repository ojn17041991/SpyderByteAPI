using SpyderByteAPI.Models;

namespace SpyderByteAPI.DataAccess.Abstract
{
    public interface IGamesAccessor
    {
        Task<IDataResponse<IList<Game>?>> GetAllAsync();

        Task<IDataResponse<Game?>> GetSingleAsync(int id);

        Task<IDataResponse<Game?>> PostAsync(Game game);

        Task<IDataResponse<Game?>> PatchAsync(int id, Game patchedGame);

        Task<IDataResponse<Game?>> DeleteAsync(int id);
    }
}
