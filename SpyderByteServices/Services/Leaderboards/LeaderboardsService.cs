using AutoMapper;
using SpyderByteDataAccess.Accessors.Leaderboards.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Leaderboards;
using SpyderByteServices.Services.Leaderboards.Abstract;

namespace SpyderByteServices.Services.Leaderboards
{
    public class LeaderboardsService(ITransactionFactory transactionFactory, ILeaderboardsAccessor leaderboardsAccessor, IMapper mapper) : ILeaderboardsService
    {
        private readonly ITransactionFactory transactionFactory = transactionFactory;
        private readonly ILeaderboardsAccessor leaderboardsAccessor = leaderboardsAccessor;
        private readonly IMapper mapper = mapper;

        public async Task<IDataResponse<Leaderboard?>> GetAsync(Guid leaderboardId)
        {
            var response = await leaderboardsAccessor.GetAsync(leaderboardId);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
        }

        public async Task<IDataResponse<Leaderboard?>> PostAsync(PostLeaderboard leaderboard)
        {
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await leaderboardsAccessor.PostAsync(mapper.Map<SpyderByteDataAccess.Models.Leaderboards.PostLeaderboard>(leaderboard));
                if (response.Result == ModelResult.Created)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
                }
            }
        }

        public async Task<IDataResponse<LeaderboardRecord?>> PostRecordAsync(PostLeaderboardRecord leaderboardRecord)
        {
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await leaderboardsAccessor.PostRecordAsync(mapper.Map<SpyderByteDataAccess.Models.Leaderboards.PostLeaderboardRecord>(leaderboardRecord));
                if (response.Result == ModelResult.Created)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>>(response);
                }
            }
        }

        public async Task<IDataResponse<Leaderboard?>> PatchAsync(PatchLeaderboard leaderboard)
        {
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await leaderboardsAccessor.PatchAsync(mapper.Map<SpyderByteDataAccess.Models.Leaderboards.PatchLeaderboard>(leaderboard));
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
                }
            }
        }

        public async Task<IDataResponse<Leaderboard?>> DeleteAsync(Guid id)
        {
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await leaderboardsAccessor.DeleteAsync(id);
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>>(response);
                }
            }
        }

        public async Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id)
        {
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await leaderboardsAccessor.DeleteRecordAsync(id);
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>>(response);
                }
            }
        }
    }
}
