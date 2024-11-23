using FluentAssertions.Execution;
using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;
using SpyderByteResources.Responses.Abstract;
using SpyderByteDataAccess.Models.Leaderboards;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests
{
    public class DeleteAsyncTests
    {
        private readonly LeaderboardsAccessorHelper _helper;
        private readonly LeaderboardsAccessorExceptionHelper _exceptionHelper;

        public DeleteAsyncTests()
        {
            _helper = new LeaderboardsAccessorHelper();
            _exceptionHelper = new LeaderboardsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_Leaderboard_In_Accessor()
        {
            // Arrange
            var storedLeaderboard = await _helper.AddLeaderboardWithRecords(3);
            var preTestLeaderboards = await _helper.GetLeaderboards();
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();

            // Act
            var returnedLeaderboard = await _helper.Accessor.DeleteAsync(storedLeaderboard.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.OK);
                returnedLeaderboard.Data.Should().NotBeNull();
                returnedLeaderboard.Data!.Id.Should().Be(storedLeaderboard.Id);

                // Check the database.
                var postTestLeaderboards = await _helper.GetLeaderboards();
                postTestLeaderboards.Should().HaveCount(preTestLeaderboards.Count - 1);

                var postTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
                postTestLeaderboards.Should().HaveCount(preTestLeaderboardRecords.Count - storedLeaderboard.LeaderboardRecords.Count());
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Leaderboard_In_Accessor_If_Leaderboard_Does_Not_Exist()
        {
            // Arrange

            // Act
            var returnedLeaderboard = await _helper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboard.Should().NotBeNull();
                returnedLeaderboard.Result.Should().Be(ModelResult.NotFound);
                returnedLeaderboard.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var storedLeaderboard = await _helper.AddLeaderboardWithRecords(3);

            // Act
            Func<Task<IDataResponse<Leaderboard?>>> func = () => _exceptionHelper.Accessor.DeleteAsync(storedLeaderboard.Id);

            // Assert
            using (new AssertionScope())
            {
                var games = await func.Invoke();
                games?.Should().NotBeNull();
                games?.Result.Should().Be(ModelResult.Error);
                games?.Data?.Should().BeNull();
            }
        }
    }
}
