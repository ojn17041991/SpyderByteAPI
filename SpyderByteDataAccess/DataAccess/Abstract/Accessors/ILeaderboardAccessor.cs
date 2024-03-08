using SpyderByteAPI.Models.Leaderboard;

namespace SpyderByteAPI.DataAccess.Abstract.Accessors
{
    public interface ILeaderboardAccessor
    {
        Task<IDataResponse<Leaderboard?>> GetAsync(Guid leaderboardId);

        Task<IDataResponse<Leaderboard?>> PostAsync(PostLeaderboard leaderboard);

        Task<IDataResponse<LeaderboardRecord?>> PostRecordAsync(PostLeaderboardRecord leaderboardRecord);

        Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id);
    }
}
