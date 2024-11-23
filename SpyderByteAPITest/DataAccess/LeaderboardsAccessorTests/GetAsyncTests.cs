using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using System.Diagnostics;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests
{
    public class GetAsyncTests
    {
        private readonly LeaderboardsAccessorHelper _helper;
        private readonly LeaderboardsAccessorExceptionHelper _exceptionHelper;

        public GetAsyncTests()
        {
            _helper = new LeaderboardsAccessorHelper();
            _exceptionHelper = new LeaderboardsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Leaderboard_Records_From_Accessor()
        {
            // Arrange
            var storedLeaderboard = await _helper.AddLeaderboardWithRecords(3);

            // Act
            var returnedLeaderboard = await _helper.Accessor.GetAsync(storedLeaderboard.Id);

            // Assert
            using (new AssertionScope())
            {
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.OK);
                returnedLeaderboard.Data.Should().NotBeNull();
                returnedLeaderboard.Data.Should().BeEquivalentTo
                (
                    storedLeaderboard,
                    options => options
                        .Excluding(l => l.LeaderboardGame)
                        .Excluding(l => l.LeaderboardRecords)
                );
                returnedLeaderboard.Data!.LeaderboardRecords.Should().BeEquivalentTo
                (
                    storedLeaderboard.LeaderboardRecords,
                    options => options
                        .Excluding(lr => lr.Leaderboard)
                );
            }
        }

        [Fact]
        public async Task Can_Not_Get_Leaderboard_Records_From_Accessor_With_Invalid_Id()
        {
            // Arrange
            Guid leaderboardId = Guid.NewGuid();

            // Act
            var returnedLeaderboard = await _helper.Accessor.GetAsync(leaderboardId);

            // Assert
            using (new AssertionScope())
            {
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.NotFound);
                returnedLeaderboard.Data.Should().BeNull();
            }
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task All_Leaderboard_Records_Are_Returned_Within_Expected_Time_Frame()
        {
            // Arrange
            const int numRecords = 1000;
            const int thresholdMs = 1000;

            var storedLeaderboard = await _helper.AddLeaderboardWithRecords(numRecords);

            // Act
            Stopwatch stopWatch = Stopwatch.StartNew();
            var returnedLeaderboard = await _helper.Accessor.GetAsync(storedLeaderboard.Id);
            stopWatch.Stop();

            // Assert
            using (new AssertionScope())
            {
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.OK);
                returnedLeaderboard.Data.Should().NotBeNull();
                returnedLeaderboard.Data!.LeaderboardRecords.Should().HaveCount(numRecords);
                stopWatch.ElapsedMilliseconds.Should().BeLessThan(thresholdMs);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<Leaderboard?>>> func = () => _exceptionHelper.Accessor.GetAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                var leaderboardRecords = await func.Invoke();
                leaderboardRecords?.Should().NotBeNull();
                leaderboardRecords?.Result.Should().Be(ModelResult.Error);
                leaderboardRecords?.Data?.Should().BeNull();
            }
        }
    }
}
