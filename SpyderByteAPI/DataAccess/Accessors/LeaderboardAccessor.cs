using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Models.Jams;
using SpyderByteAPI.Models.Leaderboard;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class LeaderboardAccessor : ILeaderboardAccessor
    {
        private ApplicationDbContext context;
        private ILogger<LeaderboardAccessor> logger;

        public LeaderboardAccessor(ApplicationDbContext context, ILogger<LeaderboardAccessor> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IDataResponse<IList<LeaderboardRecord>?>> GetAsync(Guid gameId)
        {
            try
            {
                IList<LeaderboardRecord>? data = await context.LeaderboardRecords.Where(lr => lr.GameId == gameId).OrderByDescending(lr => lr.Score).ToListAsync();
                return new DataResponse<IList<LeaderboardRecord>?>(data, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to get leaderboard records for game ID {gameId}.", e);
                return new DataResponse<IList<LeaderboardRecord>?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<LeaderboardRecord?>> PostAsync(PostLeaderboardRecord leaderboardRecord)
        {
            try
            {
                Game? game = await context.Games.SingleOrDefaultAsync(g => g.Id == leaderboardRecord.GameId);
                if (game == null)
                {
                    Jam? jam = await context.Jams.SingleOrDefaultAsync(j => j.Id == leaderboardRecord.GameId);
                    if (jam == null)
                    {
                        logger.LogInformation($"Unable to post leaderboard entry. Could not find a game/jam of ID {leaderboardRecord.GameId}.");
                        return new DataResponse<LeaderboardRecord?>(null, ModelResult.NotFound);
                    }
                }

                LeaderboardRecord mappedLeaderboardRecord = new LeaderboardRecord
                {
                    GameId = leaderboardRecord.GameId,
                    Player = leaderboardRecord.Player,
                    Score = leaderboardRecord.Score,
                    Timestamp = leaderboardRecord.Timestamp ?? DateTime.UtcNow
                };

                await context.LeaderboardRecords.AddAsync(mappedLeaderboardRecord);
                await context.SaveChangesAsync();

                return new DataResponse<LeaderboardRecord?>(mappedLeaderboardRecord, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post leaderboard record.", e);
                return new DataResponse<LeaderboardRecord?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<LeaderboardRecord?>> DeleteAsync(Guid id)
        {
            try
            {
                LeaderboardRecord? leaderboardRecord = await context.LeaderboardRecords.SingleOrDefaultAsync(lr => lr.Id == id);
                if (leaderboardRecord == null)
                {
                    logger.LogInformation($"Unable to delete leaderboard entry. Could not find a leaderboard entry of ID {id}.");
                    return new DataResponse<LeaderboardRecord?>(null, ModelResult.NotFound);
                }

                context.LeaderboardRecords.Remove(leaderboardRecord);
                await context.SaveChangesAsync();

                return new DataResponse<LeaderboardRecord?>(leaderboardRecord, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to delete leaderboard record.", e);
                return new DataResponse<LeaderboardRecord?>(null, ModelResult.Error);
            }
        }
    }
}
