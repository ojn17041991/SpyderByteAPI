﻿using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Services.Imgur.Abstract;

namespace SpyderByteAPI.DataAccess.Accessors
{
    public class GamesAccessor : IGamesAccessor
    {
        private ApplicationDbContext context;
        private ILogger<GamesAccessor> logger;
        private IConfiguration configuration;
        private IImgurService imgurService;

        public GamesAccessor(ApplicationDbContext context, ILogger<GamesAccessor> logger, IConfiguration configuration, IImgurService imgurService)
        {
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
            this.imgurService = imgurService;
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

        public async Task<IDataResponse<Game?>> GetSingleAsync(Guid id)
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
                if (game.Image == null)
                {
                    return new DataResponse<Game?>(null, ModelResult.RequestDataIncomplete);
                }

                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Name.ToLower() == game.Name.ToLower());
                if (storedGame != null)
                {
                    return new DataResponse<Game?>(storedGame, ModelResult.AlreadyExists);
                }

                var response = await imgurService.PostImageAsync(game.Image, configuration["Imgur:GamesAlbumHash"] ?? string.Empty, Path.GetFileNameWithoutExtension(game.Image.FileName));
                if (response.Result != ModelResult.OK)
                {
                    return new DataResponse<Game?>(null, response.Result);
                }

                Game mappedGame = new Game
                {
                    Name = game.Name,
                    ImgurUrl = response.Data.Url,
                    ImgurImageId = response.Data.ImageId,
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
                Game? storedGame = await context.Games.SingleOrDefaultAsync(g => g.Id == patchedGame.Id);
                if (storedGame == null)
                {
                    return new DataResponse<Game?>(storedGame, ModelResult.NotFound);
                }

                // First, check if the image is being updated.
                if (patchedGame?.Image != null)
                {
                    // Delete the old image from Imgur.
                    var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(storedGame.ImgurImageId);
                    if (!imgurDeleteSuccessful.Data)
                    {
                        logger.LogWarning($"Failed to delete image from Imgur account for game {storedGame.Name}. Continuing to database update.");
                    }

                    // Post the new image.
                    var response = await imgurService.PostImageAsync(patchedGame.Image, configuration["Imgur:GamesAlbumHash"] ?? string.Empty, Path.GetFileNameWithoutExtension(patchedGame.Image.FileName));
                    if (response.Result != ModelResult.OK)
                    {
                        return new DataResponse<Game?>(null, response.Result);
                    }

                    storedGame.ImgurUrl = response.Data.Url;
                    storedGame.ImgurImageId = response.Data.ImageId;
                }

                if (patchedGame?.Name != null && patchedGame.Name != string.Empty)
                {
                    storedGame.Name = patchedGame.Name;
                }

                if (patchedGame?.HtmlUrl != null && patchedGame.HtmlUrl != string.Empty)
                {
                    storedGame.HtmlUrl = patchedGame.HtmlUrl;
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

        public async Task<IDataResponse<Game?>> DeleteAsync(Guid id)
        {
            try
            {
                Game? game = await context.Games.SingleOrDefaultAsync(g => g.Id == id);
                if (game == null)
                {
                    return new DataResponse<Game?>(game, ModelResult.NotFound);
                }

                var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(game.ImgurImageId);
                if (!imgurDeleteSuccessful.Data)
                {
                    logger.LogWarning($"Failed to delete image from Imgur account for game {game.Name}. Continuing to database deletion.");
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

                foreach (var game in games)
                {
                    var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(game.ImgurImageId);
                    if (!imgurDeleteSuccessful.Data)
                    {
                        logger.LogWarning($"Failed to delete image from Imgur account for game {game.Name}. Continuing to database deletion.");
                    }
                }

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