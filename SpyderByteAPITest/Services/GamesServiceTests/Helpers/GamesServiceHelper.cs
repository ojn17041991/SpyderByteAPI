using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Paging;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Services.Games;
using SpyderByteServices.Services.Imgur.Abstract;

namespace SpyderByteTest.Services.GamesServiceTests.Helpers
{
    public class GamesServiceHelper
    {
        public GamesService Service;

        private const string BAD_IMGUR_ID = "BAD_IMGUR_ID";

        private readonly Fixture _fixture;
        private readonly IMapper _mapper;
        private readonly IList<SpyderByteDataAccess.Models.Games.Game> _games;

        public GamesServiceHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            _games = new List<SpyderByteDataAccess.Models.Games.Game>();

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

            var imgurService = new Mock<IImgurService>();
            imgurService.Setup(s =>
                s.PostImageAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
            )).Returns((IFormFile image, string albumHash, string title) =>
                Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Imgur.PostImageResponse>(
                        _fixture.Create<SpyderByteServices.Models.Imgur.PostImageResponse>(),
                        ModelResult.Created
                    )
                    as IDataResponse<SpyderByteServices.Models.Imgur.PostImageResponse>
                )
            );
            imgurService.Setup(s =>
                s.DeleteImageAsync(
                    It.IsAny<string>()
            )).Returns((string imageId) =>
            {
                var result = imageId != BAD_IMGUR_ID;
                return Task.FromResult(
                    new DataResponse<bool>(
                        result,
                        result ? ModelResult.OK : ModelResult.Error
                    )
                    as IDataResponse<bool>
                );
            });

            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddProfile<SpyderByteResources.Mappers.MapperProfile>();
                    config.AddProfile<SpyderByteServices.Mappers.MapperProfile>();
                }
            );
            _mapper = new Mapper(mapperConfiguration);

            var logger = new Mock<ILogger<GamesService>>();

            var configurationContents = new Dictionary<string, string?>();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationContents)
                .Build();

            Service = new GamesService(gamesAccessor.Object, imgurService.Object, _mapper, logger.Object, configuration);
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

        public void IncludeBadImgurId(Guid id)
        {
            var game = _games.Single(g => g.Id == id);

            game.ImgurImageId = BAD_IMGUR_ID;
        }
    }
}
