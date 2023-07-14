using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Models.Jams;

namespace SpyderByteAPI.DataAccess.Abstract.Accessors
{
    public interface IJamsAccessor
    {
        Task<IDataResponse<IList<Jam>?>> GetAllAsync();

        Task<IDataResponse<Jam?>> GetSingleAsync(int id);

        Task<IDataResponse<Jam?>> PostAsync(PostJam jam);

        Task<IDataResponse<Jam?>> PatchAsync(PatchJam patchedJam);

        Task<IDataResponse<Jam?>> DeleteAsync(int id);

        Task<IDataResponse<IList<Jam>?>> DeleteAllAsync();
    }
}
