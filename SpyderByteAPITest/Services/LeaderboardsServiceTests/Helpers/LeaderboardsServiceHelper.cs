using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using SpyderByteDataAccess.Accessors.Leaderboards.Abstract;
using SpyderByteServices.Services.Leaderboards;

namespace SpyderByteTest.Services.LeaderboardsServiceTests.Helpers
{
    public class LeaderboardsServiceHelper
    {
        public LeaderboardsService Service;

        private readonly Fixture _fixture;
        private readonly IMapper _mapper;
        private readonly IList<SpyderByteDataAccess.Models.Games.Game> _games;
        private readonly IList<SpyderByteDataAccess.Models.Leaderboards.Leaderboard> _leaderboards;
        private readonly IList<SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord> _leaderboardRecords;

        public LeaderboardsServiceHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            _games = new List<SpyderByteDataAccess.Models.Games.Game>();
            _leaderboards = new List<SpyderByteDataAccess.Models.Leaderboards.Leaderboard>();
            _leaderboardRecords = new List<SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord>();

            var leaderboardsAccessor = new Mock<ILeaderboardsAccessor>();

            var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<SpyderByteServices.Mappers.MapperProfile>());
            _mapper = new Mapper(mapperConfiguration);

            Service = new LeaderboardsService(leaderboardsAccessor.Object, _mapper);
        }
    }
}
