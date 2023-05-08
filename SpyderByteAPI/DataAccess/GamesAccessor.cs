using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;

namespace SpyderByteAPI.DataAccess
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
                IList<Game>? data = await context.Games.ToListAsync();
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

        public async Task<IDataResponse<Game?>> PostAsync(Game game)
        {
            try
            {
                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Id == game.Id);
                if (storedGame != null)
                {
                    return new DataResponse<Game?>(storedGame, ModelResult.AlreadyExists);
                }

                await context.Games.AddAsync(game);
                await context.SaveChangesAsync();

                return new DataResponse<Game?>(game, ModelResult.Created);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to post game.", e);
                return new DataResponse<Game?>(null, ModelResult.Error);
            }
        }

        public async Task<IDataResponse<Game?>> PatchAsync(int id, Game patchedGame)
        {
            try
            {
                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Id == id);
                if (storedGame == null)
                {
                    return new DataResponse<Game?>(storedGame, ModelResult.NotFound);
                }

                storedGame.Name = patchedGame.Name;
                storedGame.PublishDate = patchedGame.PublishDate;
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
    }
}
