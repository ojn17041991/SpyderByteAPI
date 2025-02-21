﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Helpers.Encoding;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Games;
using SpyderByteServices.Services.Games.Abstract;
using SpyderByteServices.Services.Imgur.Abstract;

namespace SpyderByteServices.Services.Games
{
    public class GamesService(ITransactionFactory transactionFactory, IGamesAccessor gamesAccessor, IImgurService imgurService, IMapper mapper, ILogger<GamesService> logger, IConfiguration configuration) : IGamesService
    {
        private readonly ITransactionFactory transactionFactory = transactionFactory;
        private readonly IGamesAccessor gamesAccessor = gamesAccessor;
        private readonly IImgurService imgurService = imgurService;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<GamesService> logger = logger;
        private readonly IConfiguration configuration = configuration;

        public async Task<IDataResponse<IPagedList<Game>?>> GetAllAsync(string? name, GameType? type, int page, int pageSize, string? order, string? direction)
        {
            var response = await gamesAccessor.GetAllAsync(name, type, page, pageSize, order, direction);
            return mapper.Map<DataResponse<IPagedList<SpyderByteServices.Models.Games.Game>?>>(response);
        }

        public async Task<IDataResponse<Game?>> GetSingleAsync(Guid id)
        {
            var response = await gamesAccessor.GetSingleAsync(id);
            return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
        }

        public async Task<IDataResponse<Game?>> PostAsync(PostGame game)
        {
            if (game.Image == null)
            {
                logger.LogInformation("Unable to post game. Image is null.");
                return new DataResponse<Game?>(null, ModelResult.RequestDataIncomplete);
            }

            // OJN: Need a new function on the accessor to check if a game exists. This is not efficient.
            var storedGames = await gamesAccessor.GetAllAsync(string.Empty, null, 1, Int32.MaxValue, string.Empty, string.Empty);
            if (storedGames.Result != ModelResult.OK)
            {
                logger.LogInformation($"Unable to post game. Failed to check existing games for duplicates.");
                return new DataResponse<Game?>(null, ModelResult.Error);
            }

            var duplicateGame = storedGames.Data!.Items.SingleOrDefault(g => g.Name == game.Name);
            if (duplicateGame != null)
            {
                logger.LogInformation($"Unable to post game. A game of name \"{LogEncoder.Encode(game.Name)}\" already exists.");
                return new DataResponse<Game?>(mapper.Map<SpyderByteServices.Models.Games.Game>(duplicateGame), ModelResult.AlreadyExists);
            }

            var imgurResponse = await imgurService.PostImageAsync(game.Image, configuration["Imgur:GamesAlbumHash"] ?? string.Empty, Path.GetFileNameWithoutExtension(game.Image.FileName));
            if (imgurResponse.Result != ModelResult.Created)
            {
                logger.LogInformation("Unable to post game. Failed to upload image to Imgur.");
                return new DataResponse<Game?>(null, imgurResponse.Result);
            }

            var dataAccessPostGame = mapper.Map<SpyderByteDataAccess.Models.Games.PostGame>(game);
            dataAccessPostGame.ImgurUrl = imgurResponse.Data!.Url;
            dataAccessPostGame.ImgurImageId = imgurResponse.Data!.ImageId;

            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await gamesAccessor.PostAsync(dataAccessPostGame);
                if (response.Result == ModelResult.Created)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
                }
            }
        }

        public async Task<IDataResponse<Game?>> PatchAsync(PatchGame game)
        {
            // OJN: ...and here.
            var storedGames = await gamesAccessor.GetAllAsync(string.Empty, null, 1, Int32.MaxValue, string.Empty, string.Empty);
            if (storedGames.Result != ModelResult.OK)
            {
                logger.LogInformation($"Unable to patch game. Failed to check existing games for duplicates.");
                return new DataResponse<Game?>(null, ModelResult.Error);
            }

            var storedGame = storedGames.Data!.Items.SingleOrDefault(g => g.Id == game.Id);
            if (storedGame == null)
            {
                logger.LogInformation($"Unable to patch game. Could not find a game of ID {game.Id}.");
                return new DataResponse<Game?>(null, ModelResult.NotFound);
            }

            if (game.Name != null)
            {
                var duplicateGame = storedGames.Data!.Items.SingleOrDefault(g => g.Name == game.Name && g.Id != game.Id);
                if (duplicateGame != null)
                {
                    logger.LogInformation($"Unable to patch game. A game of name \"{LogEncoder.Encode(game.Name)}\" already exists.");
                    return new DataResponse<Game?>(null, ModelResult.AlreadyExists);
                }
            }

            var dataAccessPatchGame = mapper.Map<SpyderByteDataAccess.Models.Games.PatchGame>(game);

            if (game.Image != null)
            {
                // Delete the old image from Imgur.
                var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(storedGame.ImgurImageId);
                if (!imgurDeleteSuccessful.Data)
                {
                    logger.LogInformation($"Failed to delete image from Imgur during game patch. Continuing to database update.");
                }

                // Post the new image.
                var imgurResponse = await imgurService.PostImageAsync(game.Image, configuration["Imgur:GamesAlbumHash"] ?? string.Empty, Path.GetFileNameWithoutExtension(game.Image.FileName));
                if (imgurResponse.Result != ModelResult.Created)
                {
                    logger.LogInformation($"Unable to patch game. Failed to add image to Imgur.");
                    return new DataResponse<Game?>(null, imgurResponse.Result);
                }

                dataAccessPatchGame.ImgurUrl = imgurResponse.Data!.Url;
                dataAccessPatchGame.ImgurImageId = imgurResponse.Data!.ImageId;
            }

            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await gamesAccessor.PatchAsync(dataAccessPatchGame);
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
                }
            }
        }

        public async Task<IDataResponse<Game?>> DeleteAsync(Guid id)
        {
            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await gamesAccessor.DeleteAsync(id);
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();

                    var imgurDeleteSuccessful = await imgurService.DeleteImageAsync(response.Data!.ImgurImageId);
                    if (!imgurDeleteSuccessful.Data)
                    {
                        logger.LogInformation($"Failed to delete image from Imgur during game delete.");
                        return new DataResponse<Game?>(null, ModelResult.Error);
                    }

                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
                }
                else
                {
                    await transaction.RollbackAsync();
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(response);
                }
            }
        }
    }
}
