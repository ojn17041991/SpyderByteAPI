using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
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

        public async Task<IDataResponse<Leaderboard?>> GetAsync(Guid gameId)
        {
            try
            {
                Leaderboard? data = await context.Leaderboards
                    .Include(l => l.LeaderboardGame)
                    .Include(l => l.LeaderboardRecords)
                    .Where(l => l.LeaderboardGame.GameId == gameId)
                    .SingleOrDefaultAsync();
                return new DataResponse<Leaderboard?>(data, data == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to get leaderboard records for game ID {gameId}.", e);
                return new DataResponse<Leaderboard?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Leaderboard?>> PostAsync(PostLeaderboard leaderboard)
        {
            try
            {
                Game? game = await context.Games
                    .Include(g => g.LeaderboardGame)
                    .SingleOrDefaultAsync(g => g.Id == leaderboard.GameId);

                if (game == null)
                {
                    logger.LogInformation($"Unable to post leaderboard. Could not find a game of ID {leaderboard.GameId}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.NotFound);
                }

                if (game.LeaderboardGame != null)
                {
                    logger.LogInformation($"Unable to post leaderboard. A leaderboard is already assigned to game of ID {leaderboard.GameId}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.AlreadyExists);
                }

                Leaderboard mappedLeaderboard = new Leaderboard();

                await context.Leaderboards.AddAsync(mappedLeaderboard);

                LeaderboardGame mappedLeaderboardGame = new LeaderboardGame
                {
                    LeaderboardId = mappedLeaderboard.Id,
                    Leaderboard = mappedLeaderboard,
                    GameId = game.Id,
                    Game = game
                };

                await context.LeaderboardGames.AddAsync(mappedLeaderboardGame);

                await context.SaveChangesAsync();

                return new DataResponse<Leaderboard?>(mappedLeaderboard, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post leaderboard record.", e);
                return new DataResponse<Leaderboard?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<LeaderboardRecord?>> PostRecordAsync(PostLeaderboardRecord leaderboardRecord)
        {
            try
            {
                Leaderboard? leaderboard = await context.Leaderboards.SingleOrDefaultAsync(l => l.Id == leaderboardRecord.LeaderboardId);
                if (leaderboard == null)
                {
                    logger.LogInformation($"Unable to post leaderboard entry. Could not find a leaderboard of ID {leaderboardRecord.LeaderboardId}.");
                    return new DataResponse<LeaderboardRecord?>(null, ModelResult.NotFound);
                }

                LeaderboardRecord mappedLeaderboardRecord = new LeaderboardRecord
                {
                    Leaderboard = leaderboard,
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

        // OJN: Will want delete leaderboard later on... and more granularity on the claim types. DeleteRecord, PostRecord, etc.

        public async Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id)
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
