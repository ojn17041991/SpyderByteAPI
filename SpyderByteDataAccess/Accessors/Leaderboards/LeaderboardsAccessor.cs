using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpyderByteDataAccess.Accessors.Leaderboards.Abstract;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Leaderboards
{
    public class LeaderboardsAccessor(ApplicationDbContext context, ILogger<LeaderboardsAccessor> logger) : ILeaderboardsAccessor
    {
        private readonly ApplicationDbContext context = context;
        private readonly ILogger<LeaderboardsAccessor> logger = logger;

        public async Task<IDataResponse<Leaderboard?>> GetAsync(Guid id)
        {
            try
            {
                // Get the leaderboard.
                Leaderboard? leaderboard = await context.Leaderboards
                    .Include(l => l.LeaderboardGame)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(l => l.Id == id);

                if (leaderboard == null)
                {
                    logger.LogInformation($"Unable to get leaderboard. Could not find a leaderboard of ID {id}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.NotFound);
                }

                // Get the records separately.
                ICollection<LeaderboardRecord> records = await context.LeaderboardRecords
                    .Include(lr => lr.Leaderboard)
                    .AsNoTracking()
                    .Where(r => r.LeaderboardId == id)
                    .ToListAsync();

                // Join the records to the leaderboard.
                // This is quicker than letting EF core do the join.
                leaderboard.LeaderboardRecords = records;


                return new DataResponse<Leaderboard?>(leaderboard, leaderboard == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to get leaderboard records for ID {id}.");
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

                Leaderboard mappedLeaderboard = new();

                await context.Leaderboards.AddAsync(mappedLeaderboard);

                LeaderboardGame mappedLeaderboardGame = new()
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
                logger.LogError(e, "Failed to post leaderboard record.");
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

                LeaderboardRecord mappedLeaderboardRecord = new()
                {
                    LeaderboardId = leaderboard.Id,
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
                logger.LogError(e, "Failed to post leaderboard record.");
                return new DataResponse<LeaderboardRecord?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Leaderboard?>> PatchAsync(PatchLeaderboard leaderboard)
        {
            try
            {
                Leaderboard? storedLeaderboard = await context.Leaderboards.SingleOrDefaultAsync(l => l.Id == leaderboard.Id);
                if (storedLeaderboard == null)
                {
                    logger.LogInformation($"Unable to patch leaderboard. Could not find a leaderboard of ID {leaderboard.Id}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.NotFound);
                }

                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Id == leaderboard.GameId);
                if (storedGame == null)
                {
                    logger.LogInformation($"Unable to patch leaderboard. Could not find a game of ID {leaderboard.GameId}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.NotFound);
                }

                var gameAllocatedToLeaderboard = await context.LeaderboardGames.AnyAsync(lg => lg.GameId == leaderboard.GameId && lg.LeaderboardId != leaderboard.Id);
                if (gameAllocatedToLeaderboard == true)
                {
                    logger.LogInformation($"Unable to patch leaderboard. A leaderboard is already assigned to game of ID {leaderboard.GameId}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.AlreadyExists);
                }

                LeaderboardGame? storedLeaderboardGame = await context.LeaderboardGames.SingleOrDefaultAsync(lg => lg.LeaderboardId == leaderboard.Id);
                if (storedLeaderboardGame == null)
                {
                    logger.LogInformation($"Unable to patch leaderboard. Could not find an existing game associated with leaderboard {leaderboard.Id}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.NotFound);
                }

                storedLeaderboardGame.GameId = leaderboard.GameId;

                await context.SaveChangesAsync();

                return new DataResponse<Leaderboard?>(storedLeaderboard, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to patch leaderboard.");
                return new DataResponse<Leaderboard?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Leaderboard?>> DeleteAsync(Guid id)
        {
            try
            {
                Leaderboard? leaderboard = await context.Leaderboards
                    .Include(l => l.LeaderboardRecords)
                    .Include(l => l.LeaderboardGame)
                    .SingleOrDefaultAsync(l => l.Id == id);

                if (leaderboard == null)
                {
                    logger.LogInformation($"Unable to delete leaderboard. Could not find a leaderboard of ID {id}.");
                    return new DataResponse<Leaderboard?>(null, ModelResult.NotFound);
                }

                context.LeaderboardRecords.RemoveRange(leaderboard.LeaderboardRecords);
                context.LeaderboardGames.Remove(leaderboard.LeaderboardGame);
                context.Leaderboards.Remove(leaderboard);
                await context.SaveChangesAsync();

                return new DataResponse<Leaderboard?>(leaderboard, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to delete leaderboard.");
                return new DataResponse<Leaderboard?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<LeaderboardRecord?>> DeleteRecordAsync(Guid id)
        {
            try
            {
                LeaderboardRecord? leaderboardRecord = await context.LeaderboardRecords.SingleOrDefaultAsync(lr => lr.Id == id);
                if (leaderboardRecord == null)
                {
                    logger.LogInformation($"Unable to delete leaderboard record. Could not find a leaderboard record of ID {id}.");
                    return new DataResponse<LeaderboardRecord?>(null, ModelResult.NotFound);
                }

                context.LeaderboardRecords.Remove(leaderboardRecord);
                await context.SaveChangesAsync();

                return new DataResponse<LeaderboardRecord?>(leaderboardRecord, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to delete leaderboard record.");
                return new DataResponse<LeaderboardRecord?>(null, ModelResult.Error);
            }
        }
    }
}
