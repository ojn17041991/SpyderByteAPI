using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Moq;
using SpyderByteAPI.Models.Games;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteResources.Models.Paging;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Services.Games.Abstract;

namespace SpyderByteTest.API.GamesControllerTests.Helpers
{
    public class GamesControllerHelper
    {
        public SpyderByteAPI.Controllers.Games.V1.GamesController ControllerV1;
        public SpyderByteAPI.Controllers.Games.V1_3.GamesController ControllerV1_3;

        private readonly Fixture fixture;

        private bool allowUseOfNonPaginatedEndpoints = true;
        private ModelResult currentModelResult = ModelResult.OK;

        public GamesControllerHelper()
        {
            var gamesService = new Mock<IGamesService>();
            gamesService.Setup(x =>
                x.GetAllAsync(
                    It.IsAny<string?>(),
                    It.IsAny<GameType?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>()
                )
            ).Returns((string? name, GameType? type, int page, int pageSize, string? order, string? direction) =>
            {
                return Task.FromResult(
                    new DataResponse<IPagedList<SpyderByteServices.Models.Games.Game>>(
                        new PagedList<SpyderByteServices.Models.Games.Game>(
                            new List<SpyderByteServices.Models.Games.Game>
                            {
                                fixture.Create<SpyderByteServices.Models.Games.Game>()
                            },
                            1,
                            1,
                            10
                        ),
                        currentModelResult
                    )
                    as IDataResponse<IPagedList<SpyderByteServices.Models.Games.Game>?>
                );
            });
            gamesService.Setup(x =>
                x.GetSingleAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Games.Game>(
                        fixture.Create<SpyderByteServices.Models.Games.Game>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Games.Game?>
                );
            });
            gamesService.Setup(x =>
                x.PostAsync(
                    It.IsAny<SpyderByteServices.Models.Games.PostGame>()
                )
            ).Returns((SpyderByteServices.Models.Games.PostGame postGame) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Games.Game>(
                        fixture.Create<SpyderByteServices.Models.Games.Game>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Games.Game?>
                );
            });
            gamesService.Setup(x =>
                x.PatchAsync(
                    It.IsAny<SpyderByteServices.Models.Games.PatchGame>()
                )
            ).Returns((SpyderByteServices.Models.Games.PatchGame patchGame) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Games.Game>(
                        fixture.Create<SpyderByteServices.Models.Games.Game>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Games.Game?>
                );
            });
            gamesService.Setup(x =>
                x.DeleteAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Games.Game>(
                        fixture.Create<SpyderByteServices.Models.Games.Game>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Games.Game?>
                );
            });

            var featureManager = new Mock<IFeatureManager>();
            featureManager.Setup(x =>
                x.IsEnabledAsync(
                    It.IsAny<string>()
                )
            ).Returns((string featureName) =>
            {
                if (featureName == FeatureFlags.AllowUseOfNonPaginatedEndpoints)
                {
                    return Task.FromResult(allowUseOfNonPaginatedEndpoints);
                }
                return Task.FromResult(false);
            });

            var modelResources = new Mock<IStringLookup<ModelResult>>();
            modelResources.Setup(x =>
                x.GetResource(
                    It.IsAny<ModelResult>()
                )
            ).Returns((ModelResult modelResult) => {
                return string.Empty;
            });

            fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddProfile<SpyderByteResources.Mappers.MapperProfile>();
                    config.AddProfile<SpyderByteAPI.Mappers.MapperProfile>();
                }
            );
            var mapper = new Mapper(mapperConfiguration);

            ControllerV1 = new(gamesService.Object, featureManager.Object, mapper, modelResources.Object);
            ControllerV1_3 = new(gamesService.Object, mapper);
        }

        public void SetCurrentModelResult(ModelResult currentModelResult)
        {
            this.currentModelResult = currentModelResult;
        }

        public PostGame GeneratePostGame()
        {
            return fixture.Create<PostGame>();
        }

        public PatchGame GeneratePatchGame()
        {
            return fixture.Create<PatchGame>();
        }
        public void SetAllowUseOfNonPaginatedEndpoints(bool allowUseOfNonPaginatedEndpoints)
        {
            this.allowUseOfNonPaginatedEndpoints = allowUseOfNonPaginatedEndpoints;
        }
    }
}
