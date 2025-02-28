using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Leaderboards.Abstract
{
    public interface ILeaderboardsAccessor
    {
        Task<IDataResponse<Leaderboard?>> GetAsync(Guid leaderboardId);

        Task<IDataResponse<Leaderboard?>> PostAsync(PostLeaderboard leaderboard);

        Task<IDataResponse<LeaderboardRecord?>> PostRecordAsync(PostLeaderboardRecord leaderboardRecord);

        Task<IDataResponse<Leaderboard?>> PatchAsync(PatchLeaderboard leaderboard);

        Task<IDataResponse<Leaderboard?>> DeleteAsync(Guid id);

        Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id);
    }
}
