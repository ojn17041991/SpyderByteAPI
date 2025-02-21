using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Leaderboards;

namespace SpyderByteServices.Services.Leaderboards.Abstract
{
    public interface ILeaderboardsService
    {
        Task<IDataResponse<Leaderboard?>> GetAsync(Guid leaderboardId);

        Task<IDataResponse<Leaderboard?>> PostAsync(PostLeaderboard leaderboard);

        Task<IDataResponse<LeaderboardRecord?>> PostRecordAsync(PostLeaderboardRecord leaderboardRecord);

        Task<IDataResponse<Leaderboard?>> PatchAsync(PatchLeaderboard leaderboard);

        Task<IDataResponse<Leaderboard?>> DeleteAsync(Guid id);

        Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id);
    }
}
