using AutoFixture;
using AutoMapper;
using Moq;
using SpyderByteAPI.Controllers;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteServices.Services.Leaderboards.Abstract;
using SpyderByteServices.Services.Authorization.Abstract;
using SpyderByteTest.API.LeaderboardsControllerTests.Mocks;
using SpyderByteAPI.Models.Leaderboards;

namespace SpyderByteTest.API.LeaderboardsControllerTests.Helpers
{
    public class LeaderboardsControllerHelper
    {
        public LeaderboardsController Controller;

        private readonly Fixture fixture;

        private bool authorizeRequests = true;
        private ModelResult currentModelResult = ModelResult.OK;

        public LeaderboardsControllerHelper()
        {
            var leaderboardsService = new Mock<ILeaderboardsService>();
            leaderboardsService.Setup(x =>
                x.GetAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard>(
                        fixture.Create<SpyderByteServices.Models.Leaderboards.Leaderboard>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsService.Setup(x =>
                x.PostAsync(
                    It.IsAny<SpyderByteServices.Models.Leaderboards.PostLeaderboard>()
                )
            ).Returns((SpyderByteServices.Models.Leaderboards.PostLeaderboard postLeaderboard) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard>(
                        fixture.Create<SpyderByteServices.Models.Leaderboards.Leaderboard>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsService.Setup(x =>
                x.PostRecordAsync(
                    It.IsAny<SpyderByteServices.Models.Leaderboards.PostLeaderboardRecord>()
                )
            ).Returns((SpyderByteServices.Models.Leaderboards.PostLeaderboardRecord postLeaderboardRecord) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord>(
                        fixture.Create<SpyderByteServices.Models.Leaderboards.LeaderboardRecord>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>
                );
            });
            leaderboardsService.Setup(x =>
                x.PatchAsync(
                    It.IsAny<SpyderByteServices.Models.Leaderboards.PatchLeaderboard>()
                )
            ).Returns((SpyderByteServices.Models.Leaderboards.PatchLeaderboard patchLeaderboard) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard>(
                        fixture.Create<SpyderByteServices.Models.Leaderboards.Leaderboard>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsService.Setup(x =>
                x.DeleteAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard>(
                        fixture.Create<SpyderByteServices.Models.Leaderboards.Leaderboard>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsService.Setup(x =>
                x.DeleteRecordAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord>(
                        fixture.Create<SpyderByteServices.Models.Leaderboards.LeaderboardRecord>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Leaderboards.LeaderboardRecord?>
                );
            });

            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x =>
                x.UserHasAccessToLeaderboard(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()
                )
            ).Returns((Guid userId, Guid leaderboardId) =>
            {
                return Task.FromResult(
                    new DataResponse<bool>(
                        authorizeRequests,
                        authorizeRequests ? ModelResult.OK : ModelResult.Unauthorized
                    )
                    as IDataResponse<bool>
                );
            });

            var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<SpyderByteAPI.Mappers.MapperProfile>());
            var mapper = new Mapper(mapperConfiguration);

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

            Controller = new LeaderboardsController(leaderboardsService.Object, authorizationService.Object, mapper, modelResources.Object);
            Controller.ControllerContext = new MockControllerContext();
        }

        public void SetCurrentModelResult(ModelResult currentModelResult)
        {
            this.currentModelResult = currentModelResult;
        }

        public void SetAuthorizeRequests(bool authorizeRequests)
        {
            this.authorizeRequests = authorizeRequests;
        }

        public PostLeaderboard GeneratePostLeaderboard()
        {
            return fixture.Create<PostLeaderboard>();
        }

        public PostLeaderboardRecord GeneratePostRecord()
        {
            return fixture.Create<PostLeaderboardRecord>();
        }

        public PatchLeaderboard GeneratePatchLeaderboard()
        {
            return fixture.Create<PatchLeaderboard>();
        }
    }
}
