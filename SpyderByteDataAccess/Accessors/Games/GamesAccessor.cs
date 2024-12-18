﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteDataAccess.Accessors.Games
{
    public class GamesAccessor(ApplicationDbContext context, ILogger<GamesAccessor> logger) : IGamesAccessor
    {
        private readonly ApplicationDbContext context = context;
        private readonly ILogger<GamesAccessor> logger = logger;

        public async Task<IDataResponse<IList<Game>?>> GetAllAsync()
        {
            try
            {
                IList<Game>? data = await context.Games
                    .Include(g => g.UserGame)
                        .ThenInclude(ug => ug!.User)
                    .Include(g => g.LeaderboardGame)
                        .ThenInclude(lg => lg!.Leaderboard)
                    .AsNoTracking()
                    .OrderBy(g => g.PublishDate)
                    .ToListAsync();
                return new DataResponse<IList<Game>?>(data, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get all games.");
                return new DataResponse<IList<Game>?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> GetSingleAsync(Guid id)
        {
            try
            {
                Game? game = await context.Games
                    .Include(g => g.UserGame)
                        .ThenInclude(ug => ug!.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == id);
                return new DataResponse<Game?>(game, game == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get single game.");
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> PostAsync(PostGame game)
        {
            try
            {
                Game mappedGame = new()
                {
                    Name = game.Name,
                    Type = game.Type,
                    ImgurUrl = game.ImgurUrl,
                    ImgurImageId = game.ImgurImageId,
                    Url = game.Url,
                    PublishDate = game.PublishDate
                };

                await context.Games.AddAsync(mappedGame);
                await context.SaveChangesAsync();

                return new DataResponse<Game?>(mappedGame, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to post game.");
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame)
        {
            try
            {
                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Id == patchedGame.Id);
                if (storedGame == null)
                {
                    logger.LogError($"Failed to patch game. A game of ID {patchedGame.Id} does not exist.");
                    return new DataResponse<Game?>(null, ModelResult.NotFound);
                }

                if (patchedGame.ImgurUrl.IsNullOrEmpty() == false)
                {
                    storedGame.ImgurUrl = patchedGame.ImgurUrl!;
                }

                if (patchedGame.ImgurImageId.IsNullOrEmpty() == false)
                {
                    storedGame.ImgurImageId = patchedGame.ImgurImageId!;
                }

                if (patchedGame.Name.IsNullOrEmpty() == false)
                {
                    storedGame.Name = patchedGame.Name!;
                }

                if (patchedGame.Type != null)
                {
                    storedGame.Type = patchedGame.Type.Value;
                }

                if (patchedGame.Url.IsNullOrEmpty() == false)
                {
                    storedGame.Url = patchedGame.Url!;
                }

                if (patchedGame.PublishDate != null)
                {
                    storedGame.PublishDate = patchedGame.PublishDate.Value;
                }

                await context.SaveChangesAsync();

                return new DataResponse<Game?>(storedGame, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to patch game.");
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> DeleteAsync(Guid id)
        {
            try
            {
                Game? game = await context.Games
                    .Include(g => g.LeaderboardGame)
                    .Include(g => g.UserGame)
                    .SingleOrDefaultAsync(g => g.Id == id);

                if (game == null)
                {
                    logger.LogError($"Failed to delete game. A game of ID {id} does not exist.");
                    return new DataResponse<Game?>(null, ModelResult.NotFound);
                }

                if (game.LeaderboardGame != null || game.UserGame != null)
                {
                    logger.LogError($"Failed to delete game. A game of ID {id} does not exist.");
                    return new DataResponse<Game?>(game, ModelResult.RelationshipViolation);
                }

                context.Games.Remove(game);
                await context.SaveChangesAsync();

                return new DataResponse<Game?>(game, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to delete game.");
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }
    }
}
