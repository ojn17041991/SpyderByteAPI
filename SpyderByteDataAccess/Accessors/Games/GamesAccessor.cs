using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    public class GamesAccessor : IGamesAccessor
    {
        private ApplicationDbContext context;
        private ILogger<GamesAccessor> logger;
        private IConfiguration configuration;

        public GamesAccessor(ApplicationDbContext context, ILogger<GamesAccessor> logger, IConfiguration configuration)
        {
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<IDataResponse<IList<Game>?>> GetAllAsync()
        {
            try
            {
                IList<Game>? data = await context.Games
                    .Include(g => g.UserGame)
                        .ThenInclude(ug => ug!.User)
                    .Include(g => g.LeaderboardGame)
                        .ThenInclude(lg => lg!.Leaderboard)
                    .OrderBy(g => g.PublishDate).ToListAsync();
                return new DataResponse<IList<Game>?>(data, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get all games.", e);
                return new DataResponse<IList<Game>?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> GetSingleAsync(Guid id)
        {
            try
            {
                Game? game = await context.Games
                    .Include(j => j.UserGame)
                        .ThenInclude(uj => uj!.User)
                    .SingleOrDefaultAsync(j => j.Id == id);
                return new DataResponse<Game?>(game, game == null ? ModelResult.NotFound : ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get single game.", e);
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> PostAsync(PostGame game)
        {
            try
            {
                // OJN: Rewrite to use mapper later.
                Game mappedGame = new Game
                {
                    Name = game.Name,
                    Type = game.Type,
                    ImgurUrl = game.ImgurUrl,
                    ImgurImageId = game.ImgurImageId,
                    HtmlUrl = game.HtmlUrl,
                    PublishDate = game.PublishDate
                };

                await context.Games.AddAsync(mappedGame);
                await context.SaveChangesAsync();

                return new DataResponse<Game?>(mappedGame, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post game.", e);
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> PatchAsync(PatchGame patchedGame)
        {
            try
            {
                Game storedGame = await context.Games.SingleAsync(g => g.Id == patchedGame.Id);

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

                if (patchedGame.HtmlUrl.IsNullOrEmpty() == false)
                {
                    storedGame.HtmlUrl = patchedGame.HtmlUrl!;
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
                logger.LogError("Failed to patch game.", e);
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> DeleteAsync(Guid id)
        {
            try
            {
                Game game = await context.Games
                    .Include(g => g.UserGame)
                    .SingleAsync(g => g.Id == id);

                context.Games.Remove(game);
                await context.SaveChangesAsync();

                return new DataResponse<Game?>(game, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to delete game.", e);
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }
    }
}
