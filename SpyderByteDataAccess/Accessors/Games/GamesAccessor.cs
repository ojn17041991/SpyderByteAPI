using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Models.Games;
using SpyderByteDataAccess.Paging.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using System.Linq.Expressions;

namespace SpyderByteDataAccess.Accessors.Games
{
    public class GamesAccessor(ApplicationDbContext context, IPagedListFactory pagedListFactory, ILogger<GamesAccessor> logger) : IGamesAccessor
    {
        private readonly ApplicationDbContext context = context;
        private readonly IPagedListFactory pagedListFactory = pagedListFactory;
        private readonly ILogger<GamesAccessor> logger = logger;

        public async Task<IDataResponse<IPagedList<Game>?>> GetAllAsync(string? name, GameType? type, int page, int pageSize, string? order, string? direction)
        {
            try
            {
                // Get all games.
                IQueryable<Game> query = context.Games
                    .Include(g => g.UserGame)
                        .ThenInclude(ug => ug!.User)
                    .Include(g => g.LeaderboardGame)
                        .ThenInclude(lg => lg!.Leaderboard)
                    .AsNoTracking();

                // Check if the filters are available.
                bool hasNameFilter = name != null;
                bool hasTypeFilter = type != null;

                // Lower case the name to make the search case insensitive.
                name = name?.ToLower();

                // Set up the filtering function.
                Expression<Func<Game, bool>> filteringFunction =
                    (g) =>
                        (hasTypeFilter == false || g.Type == type) &&
                        (hasNameFilter == false || g.Name.ToLower().Contains(name!));

                // Pick the ordering expression.
                Expression<Func<Game, object>> orderingFunction = order?.ToLower() switch
                {
                    "name" => (g) => g.Name.ToLower(),
                    "date" => (g) => g.PublishDate,
                    _ => (g) => g.PublishDate
                };

                // Convert to paged list.
                IPagedList<Game>? games = await pagedListFactory.BuildAsync(
                    query,
                    filteringFunction,
                    orderingFunction,
                    direction,
                    page,
                    pageSize
                );

                return new DataResponse<IPagedList<Game>?>(games, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get all games.");
                return new DataResponse<IPagedList<Game>?>(null, ModelResult.Error);
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
