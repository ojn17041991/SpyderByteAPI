using AutoFixture;
using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Paging;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Data;
using SpyderByteServices.Services.Games;
using SpyderByteServices.Services.Storage.Abstract;
using SpyderByteServices.Services.Storage.Image.Abstract;

namespace SpyderByteTest.Services.GamesServiceTests.Helpers
{
    public class GamesServiceHelper
    {
        public GamesService Service;

        private bool _failOnImageRequest = false;

        private readonly Fixture _fixture;
        private readonly IMapper _mapper;
        private readonly IList<SpyderByteDataAccess.Models.Games.Game> _games;

        public GamesServiceHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddProfile<SpyderByteResources.Mappers.MapperProfile>();
                    config.AddProfile<SpyderByteServices.Mappers.MapperProfile>();
                }
            );
            _mapper = new Mapper(mapperConfiguration);

            _games = new List<SpyderByteDataAccess.Models.Games.Game>();

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t =>
                t.CommitAsync(
                    It.IsAny<CancellationToken>()
                )
            );
            transaction.Setup(t =>
                t.RollbackAsync(
                    It.IsAny<CancellationToken>()
                )
            );

            var transactionFactory = new Mock<ITransactionFactory>();
            transactionFactory.Setup(f =>
                f.CreateAsync()
            ).Returns(
                Task.FromResult(
                    transaction.Object
                )
            );

            var gamesAccessor = new Mock<IGamesAccessor>();
            gamesAccessor.Setup(s =>
                s.GetAllAsync(
                    It.IsAny<string?>(),
                    It.IsAny<GameType?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>()
                )
            ).Returns(
                Task.FromResult(
                    new DataResponse<IPagedList<SpyderByteDataAccess.Models.Games.Game>?>(
                        new PagedList<SpyderByteDataAccess.Models.Games.Game>(
                            _games,
                            _games.Count(),
                            1,
                            10
                        ),
                        ModelResult.OK
                    )
                    as IDataResponse<IPagedList<SpyderByteDataAccess.Models.Games.Game>?>
                )
            );
            gamesAccessor.Setup(s =>
                s.GetSingleAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var game = _games.SingleOrDefault(g => g.Id == id);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Games.Game?>(
                        game,
                        game == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Games.Game?>
                );
            });
            gamesAccessor.Setup(s =>
                s.GetSingleByNameAsync(
                    It.IsAny<string>()
            )).Returns((string name) =>
            {
                var game = _games.SingleOrDefault(g => g.Name == name);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Games.Game?>(
                        game,
                        game == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Games.Game?>
                );
            });
            gamesAccessor.Setup(s =>
                s.PostAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Games.PostGame>()
            )).Returns((SpyderByteDataAccess.Models.Games.PostGame postGame) =>
            {
                var game = new SpyderByteDataAccess.Models.Games.Game
                {
                    Id = Guid.NewGuid(),
                    Name = postGame.Name,
                    Type = postGame.Type,
                    Url = postGame.Url,
                    ImgurUrl = postGame.ImgurUrl,
                    ImgurImageId = postGame.ImgurImageId,
                    PublishDate = postGame.PublishDate
                };
                _games.Add(game);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Games.Game?>(
                        game,
                        ModelResult.Created
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Games.Game?>
                );
            });
            gamesAccessor.Setup(s =>
                s.PatchAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Games.PatchGame>()
            )).Returns((SpyderByteDataAccess.Models.Games.PatchGame patchGame) =>
            {
                var game = _games.Single(g => g.Id == patchGame.Id);
                game.Name = patchGame.Name!;
                game.Type = patchGame.Type!.Value;
                game.Url = patchGame.Url!;
                game.ImgurUrl = patchGame.ImgurUrl!;
                game.ImgurImageId = patchGame.ImgurImageId!;
                game.PublishDate = patchGame.PublishDate!.Value;
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Games.Game?>(
                        game,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Games.Game?>
                );
            });
            gamesAccessor.Setup(s =>
                s.DeleteAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var game = _games.Single(g => g.Id == id);
                _games.Remove(game);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Games.Game?>(
                        game,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Games.Game?>
                );
            });

            var storageServiceBlobServiceClient = new Mock<BlobServiceClient>();

            var storageServiceClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
            storageServiceClientFactory.Setup(x =>
                x.CreateClient(
                    It.IsAny<string>()
                )
            ).Returns((string clientName) =>
            {
                return storageServiceBlobServiceClient.Object;
            });

            var storageServiceConfiguration = new Mock<IConfiguration>();
            var storageServiceLogger = new Mock<ILogger<BaseStorageService>>();

            var imageStorageService = new Mock<BaseImageStorageService>(
                storageServiceClientFactory.Object,
                storageServiceConfiguration.Object,
                _mapper,
                storageServiceLogger.Object
            );
            imageStorageService.Setup(s =>
                s.UploadAsync(
                    It.IsAny<string>(),
                    It.IsAny<Stream>()
            )).Returns((string fileName, Stream stream) =>
                Task.FromResult(
                    new DataResponse<StorageFile>(
                        _fixture.Create<StorageFile>(),
                        _failOnImageRequest ? ModelResult.Error : ModelResult.Created
                    )
                    as IDataResponse<StorageFile?>
                )
            );
            imageStorageService.Setup(s =>
                s.DeleteAsync(
                    It.IsAny<string>()
            )).Returns((string fileName) =>
            {
                return Task.FromResult(
                    new DataResponse<StorageFile>(
                        _fixture.Create<StorageFile>(),
                        _failOnImageRequest ? ModelResult.Error : ModelResult.OK
                    )
                    as IDataResponse<StorageFile?>
                );
            });

            var logger = new Mock<ILogger<GamesService>>();

            var configurationContents = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            Service = new GamesService(
                transactionFactory.Object,
                gamesAccessor.Object,
                imageStorageService.Object,
                _mapper,
                logger.Object,
                configuration
            );
        }

        public SpyderByteServices.Models.Games.Game AddGame()
        {
            var game = _fixture.Create<SpyderByteDataAccess.Models.Games.Game>();
            _games.Add(game);
            return _mapper.Map<SpyderByteServices.Models.Games.Game>(game);
        }

        public SpyderByteServices.Models.Games.PostGame GeneratePostGame()
        {
            return _fixture.Create<SpyderByteServices.Models.Games.PostGame>();
        }

        public SpyderByteServices.Models.Games.PatchGame GeneratePatchGame()
        {
            return _fixture.Create<SpyderByteServices.Models.Games.PatchGame>();
        }

        public void RemoveGameUserRelationship(Guid id)
        {
            var game = _games.Single(g => g.Id == id);

            if (game.UserGame != null)
            {
                game.UserGame = null!;
            }
        }

        public void RemoveGameLeaderboardRelationship(Guid id)
        {
            var game = _games.Single(g => g.Id == id);

            if (game.LeaderboardGame != null)
            {
                game.LeaderboardGame = null!;
            }
        }

        public void SetFailOnImageRequest(bool failOnImageRequest)
        {
            _failOnImageRequest = failOnImageRequest;
        }
    }
}
