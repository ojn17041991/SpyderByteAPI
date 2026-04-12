using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Extensions;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Games;
using SpyderByteServices.Services.Games.Abstract;
using SpyderByteServices.Services.Storage.Image.Abstract;

namespace SpyderByteServices.Services.Games
{
    public class GamesService(
        ITransactionFactory transactionFactory,
        IGamesAccessor gamesAccessor,
        BaseImageStorageService imageStorageService,
        IMapper mapper,
        ILogger<GamesService> logger,
        IConfiguration configuration
    ) : IGamesService
    {
        private readonly ITransactionFactory transactionFactory = transactionFactory;
        private readonly IGamesAccessor gamesAccessor = gamesAccessor;
        private readonly BaseImageStorageService imageStorageService = imageStorageService;
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

            var duplicateGameResponse = await gamesAccessor.GetSingleByNameAsync(game.Name);
            if (duplicateGameResponse.Result == ModelResult.Error)
            {
                logger.LogInformation($"Unable to post game. Failed to check if game of name {game.Name} already exists.");
                return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(duplicateGameResponse);
            }
            else if (duplicateGameResponse.Result == ModelResult.OK)
            {
                logger.LogInformation($"Unable to post game. Game of name {game.Name} already exists.");
                return new DataResponse<Game?>(mapper.Map<SpyderByteServices.Models.Games.Game>(duplicateGameResponse.Data), ModelResult.AlreadyExists);
            }
            
            var storageResponse = await imageStorageService.UploadAsync(game.Image.FileName, await game.Image.GetStream());
            if (storageResponse.Result != ModelResult.Created)
            {
                logger.LogInformation("Unable to post game. Failed to upload image to Storage.");
                return new DataResponse<Game?>(null, storageResponse.Result);
            }

            var dataAccessPostGame = mapper.Map<SpyderByteDataAccess.Models.Games.PostGame>(game);
            dataAccessPostGame.ImgurUrl = storageResponse.Data!.FileName; // OJN: This should be renamed to ImageUrl.
            dataAccessPostGame.ImgurImageId = string.Empty; // OJN: This is not needed any more.

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
            // Make sure we don't create games with duplicate names.
            if (game.Name != null)
            {
                var duplicateGameResponse = await gamesAccessor.GetSingleByNameAsync(game.Name);
                if (duplicateGameResponse.Result == ModelResult.Error)
                {
                    logger.LogInformation($"Unable to patch game. Failed to check if game of name {game.Name} already exists.");
                    return mapper.Map<DataResponse<SpyderByteServices.Models.Games.Game?>>(duplicateGameResponse);
                }
                else if (duplicateGameResponse.Result == ModelResult.OK)
                {
                    logger.LogInformation($"Unable to patch game. Game of name {game.Name} already exists.");
                    return new DataResponse<Game?>(mapper.Map<SpyderByteServices.Models.Games.Game>(duplicateGameResponse.Data), ModelResult.AlreadyExists);
                }
            }

            // Get the game being patched.
            var gameResponse = await gamesAccessor.GetSingleAsync(game.Id);
            if (gameResponse.Result != ModelResult.OK)
            {
                logger.LogInformation($"Unable to patch game. Could not find a game of ID {game.Id}.");
                return new DataResponse<Game?>(null, ModelResult.NotFound);
            }

            // Get the game data.
            var storedGame = gameResponse.Data!;
            var dataAccessPatchGame = mapper.Map<SpyderByteDataAccess.Models.Games.PatchGame>(game);

            // Track whether image deletion failed.
            bool imageDeletionFailed = false;

            if (game.Image != null)
            {
                // Delete the original image from storage.
                var fileName = Path.GetFileName(storedGame.ImgurUrl);
                var storageDeletionResponse = await imageStorageService.DeleteAsync(fileName);
                if (storageDeletionResponse.Result != ModelResult.OK)
                {
                    imageDeletionFailed = true;
                    logger.LogInformation($"Failed to delete image from Storage during game patch. Continuing to database update.");
                }

                // Upload the new image to storage.
                var storageUploadResponse = await imageStorageService.UploadAsync(game.Image.FileName, await game.Image.GetStream());
                if (storageUploadResponse.Result != ModelResult.Created)
                {
                    logger.LogInformation("Unable to patch game. Failed to upload image to Storage.");
                    return new DataResponse<Game?>(null, storageUploadResponse.Result);
                }

                dataAccessPatchGame.ImgurUrl = storageUploadResponse.Data!.FileName; // OJN: This should be renamed to ImageUrl.
                dataAccessPatchGame.ImgurImageId = string.Empty; // OJN: This is not needed any more.
            }

            using (var transaction = await transactionFactory.CreateAsync())
            {
                var response = await gamesAccessor.PatchAsync(dataAccessPatchGame);
                if (response.Result == ModelResult.OK)
                {
                    await transaction.CommitAsync();

                    var mappedGame = mapper.Map<SpyderByteServices.Models.Games.Game?>(response.Data);

                    if (imageDeletionFailed == true)
                    {
                        return new DataResponse<SpyderByteServices.Models.Games.Game?>(mappedGame, ModelResult.ImageDeletionFailed);
                    }
                    else
                    {
                        return new DataResponse<SpyderByteServices.Models.Games.Game?>(mappedGame, ModelResult.OK);
                    }
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

                    var fileName = Path.GetFileName(response.Data!.ImgurUrl);
                    var storageDeletionResponse = await imageStorageService.DeleteAsync(fileName);
                    if (storageDeletionResponse.Result != ModelResult.OK)
                    {
                        logger.LogInformation($"Unable to delete game. Image deletion failed.");
                        return new DataResponse<Game?>(
                            mapper.Map<SpyderByteServices.Models.Games.Game?>(response.Data),
                            ModelResult.ImageDeletionFailed
                        );
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
