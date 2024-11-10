using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

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
            var storedLeaderboard = await _helper.AddLeaderboardWithRecords();

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

        // OJN: New code path. Need a new test to cover leaderboard == null.

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
