using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class GamesAccessor : IGamesAccessor
    {
        private ApplicationDbContext context;
        private ILogger<GamesAccessor> logger;

        public GamesAccessor(ApplicationDbContext context, ILogger<GamesAccessor> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IDataResponse<IList<Game>?>> GetAllAsync()
        {
            try
            {
                IList<Game>? data = await context.Games.OrderBy(g => g.PublishDate).ToListAsync();
                return new DataResponse<IList<Game>?>(data, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get all games.", e);
                return new DataResponse<IList<Game>?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> GetSingleAsync(int id)
        {
            try
            {
                Game? game = await context.Games.SingleOrDefaultAsync(g => g.Id == id);
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
                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Name.ToLower() == game.Name.ToLower());
                if (storedGame != null)
                {
                    return new DataResponse<Game?>(storedGame, ModelResult.AlreadyExists);
                }

                Game mappedGame = new Game
                {
                    Name = game.Name,
                    PublishDate = (DateTime)game.PublishDate
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
                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Id == patchedGame.Id);
                if (storedGame == null)
                {
                    return new DataResponse<Game?>(storedGame, ModelResult.NotFound);
                }

                if (patchedGame?.Name != null && patchedGame.Name != string.Empty)
                {
                    storedGame.Name = patchedGame.Name;
                }

                if (patchedGame?.PublishDate != null)
                {
                    storedGame.PublishDate = (DateTime)patchedGame.PublishDate;
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

        public async Task<IDataResponse<Game?>> DeleteAsync(int id)
        {
            try
            {
                Game? game = await context.Games.SingleOrDefaultAsync(g => g.Id == id);
                if (game == null)
                {
                    return new DataResponse<Game?>(game, ModelResult.NotFound);
                }

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

        public async Task<IDataResponse<IList<Game>?>> DeleteAllAsync()
        {
            try
            {
                var games = await context.Games.ToListAsync();
                context.Games.RemoveRange(games);
                await context.SaveChangesAsync();

                return new DataResponse<IList<Game>?>(games, ModelResult.OK);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to clear games.", e);
                return new DataResponse<IList<Game>?>(null, ModelResult.Error);
            }
        }
    }
}
