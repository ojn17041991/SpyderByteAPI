using SpyderByteAPI.Models.Leaderboard;

namespace SpyderByteAPI.DataAccess.Abstract.Accessors
{
    public interface ILeaderboardAccessor
    {
        Task<IDataResponse<IList<LeaderboardRecord>?>> GetAsync(Guid gameId);

        Task<IDataResponse<LeaderboardRecord?>> PostAsync(PostLeaderboardRecord leaderboardRecord);

        Task<IDataResponse<LeaderboardRecord?>> DeleteAsync(Guid id);
    }
}
