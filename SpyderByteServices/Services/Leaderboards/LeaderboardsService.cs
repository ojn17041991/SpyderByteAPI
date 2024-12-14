using AutoMapper;
using SpyderByteDataAccess.Accessors.Leaderboards.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Leaderboards;
using SpyderByteServices.Services.Leaderboards.Abstract;

namespace SpyderByteServices.Services.Leaderboards
{
    public class LeaderboardsService(ILeaderboardsAccessor leaderboardsAccessor, IMapper mapper) : ILeaderboardsService
    {
        private readonly ILeaderboardsAccessor leaderboardsAccessor = leaderboardsAccessor;
        private readonly IMapper mapper = mapper;

        public async Task<IDataResponse<Leaderboard?>> GetAsync(Guid leaderboardId)
        {
            var response = await leaderboardsAccessor.GetAsync(leaderboardId);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
        }

        public async Task<IDataResponse<Leaderboard?>> PostAsync(PostLeaderboard leaderboard)
        {
            var response = await leaderboardsAccessor.PostAsync(mapper.Map<SpyderByteDataAccess.Models.Leaderboards.PostLeaderboard>(leaderboard));
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
        }

        public async Task<IDataResponse<LeaderboardRecord?>> PostRecordAsync(PostLeaderboardRecord leaderboardRecord)
        {
            var response = await leaderboardsAccessor.PostRecordAsync(mapper.Map<SpyderByteDataAccess.Models.Leaderboards.PostLeaderboardRecord>(leaderboardRecord));
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>>(response);
        }

        public async Task<IDataResponse<Leaderboard?>> PatchAsync(PatchLeaderboard leaderboard)
        {
            var response = await leaderboardsAccessor.PatchAsync(mapper.Map<SpyderByteDataAccess.Models.Leaderboards.PatchLeaderboard>(leaderboard));
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
        }

        public async Task<IDataResponse<Leaderboard?>> DeleteAsync(Guid id)
        {
            var response = await leaderboardsAccessor.DeleteAsync(id);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
        }

        public async Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id)
        {
            var response = await leaderboardsAccessor.DeleteRecordAsync(id);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>>(response);
        }
    }
}
