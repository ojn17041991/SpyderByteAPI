using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using SpyderByteDataAccess.Accessors.Leaderboards.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses;
using SpyderByteResources.Models.Responses.Abstract;
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

            var leaderboardsAccessor = new Mock<ILeaderboardsAccessor>();
            leaderboardsAccessor.Setup(s =>
                s.GetAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var leaderboard = _leaderboards.Single(l => l.Id == id);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>(
                        leaderboard,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsAccessor.Setup(s =>
                s.PostAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Leaderboards.PostLeaderboard>()
            )).Returns((SpyderByteDataAccess.Models.Leaderboards.PostLeaderboard postLeaderboard) =>
            {
                var leaderboard = _fixture.Create<SpyderByteDataAccess.Models.Leaderboards.Leaderboard>();
                leaderboard.LeaderboardGame.GameId = postLeaderboard.GameId;
                leaderboard.LeaderboardGame.Game.Id = postLeaderboard.GameId;
                _leaderboards.Add(leaderboard);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>(
                        leaderboard,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsAccessor.Setup(s =>
                s.PostRecordAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Leaderboards.PostLeaderboardRecord>()
            )).Returns((SpyderByteDataAccess.Models.Leaderboards.PostLeaderboardRecord postLeaderboardRecord) =>
            {
                var leaderboard = _leaderboards.Single(l => l.Id == postLeaderboardRecord.LeaderboardId);
                var leaderboardRecord = new SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord
                {
                    LeaderboardId = leaderboard.Id,
                    Leaderboard = leaderboard,
                    Player = postLeaderboardRecord.Player,
                    Score = postLeaderboardRecord.Score,
                    Timestamp = postLeaderboardRecord.Timestamp!.Value
                };
                leaderboard.LeaderboardRecords.Add(leaderboardRecord);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord?>(
                        leaderboardRecord,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord?>
                );
            });
            leaderboardsAccessor.Setup(s =>
                s.PatchAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Leaderboards.PatchLeaderboard>()
            )).Returns((SpyderByteDataAccess.Models.Leaderboards.PatchLeaderboard patchLeaderboard) =>
            {
                var leaderboard = _leaderboards.Single(l => l.Id == patchLeaderboard.Id);
                leaderboard.LeaderboardGame.GameId = patchLeaderboard.GameId;
                leaderboard.LeaderboardGame.Game.Id = patchLeaderboard.GameId;
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>(
                        leaderboard,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsAccessor.Setup(s => 
                s.DeleteAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) => 
            {
                var leaderboard = _leaderboards.Single(l => l.Id == id);
                _leaderboards.Remove(leaderboard);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>(
                        leaderboard,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Leaderboards.Leaderboard?>
                );
            });
            leaderboardsAccessor.Setup(s => 
                s.DeleteRecordAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) => 
            {
                var leaderboard = _leaderboards.Single(l => l.LeaderboardRecords.Any(lr => lr.Id == id));
                var leaderboardRecord = leaderboard.LeaderboardRecords.Single(lr => lr.Id == id);
                leaderboard.LeaderboardRecords.Remove(leaderboardRecord);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord?>(
                        leaderboardRecord,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Leaderboards.LeaderboardRecord?>
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

            Service = new LeaderboardsService(transactionFactory.Object, leaderboardsAccessor.Object, _mapper);
        }

        public SpyderByteDataAccess.Models.Leaderboards.Leaderboard AddLeaderboard()
        {
            var leaderboard = _fixture.Create<SpyderByteDataAccess.Models.Leaderboards.Leaderboard>();
            _leaderboards.Add(leaderboard);
            return leaderboard;
        }

        public SpyderByteServices.Models.Leaderboards.PostLeaderboard GeneratePostLeaderboard()
        {
            return _fixture.Create<SpyderByteServices.Models.Leaderboards.PostLeaderboard>();
        }

        public SpyderByteServices.Models.Leaderboards.PostLeaderboardRecord GeneratePostLeaderboardRecord()
        {
            return _fixture.Create<SpyderByteServices.Models.Leaderboards.PostLeaderboardRecord>();
        }

        public SpyderByteServices.Models.Leaderboards.PatchLeaderboard GeneratePatchLeaderboard()
        {
            return _fixture.Create<SpyderByteServices.Models.Leaderboards.PatchLeaderboard>();
        }
    }
}
