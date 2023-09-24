using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Leaderboard;
using SpyderByteAPITest.DataAccess.LeaderboardAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.LeaderboardAccessorTests
{
    public class DeleteAsyncTests
    {
        private readonly LeaderboardAccessorHelper _helper;
        private readonly LeaderboardAccessorExceptionHelper _exceptionHelper;

        public DeleteAsyncTests()
        {
            _helper = new LeaderboardAccessorHelper();
            _exceptionHelper = new LeaderboardAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_Leaderboard_Record_In_Accessor()
        {
            // Arrange
            var dbLeaderboardRecords = await _helper.AddLeaderboardRecord();
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();

            // Act
            var leaderboardRecords = await _helper.Accessor.DeleteAsync(dbLeaderboardRecords.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                leaderboardRecords.Should().NotBeNull();
                leaderboardRecords.Result.Should().Be(ModelResult.OK);
                leaderboardRecords.Data.Should().BeEquivalentTo(dbLeaderboardRecords);

                // Check the database.
                var postTestLeaderboards = await _helper.GetLeaderboardRecords();
                postTestLeaderboards.Should().HaveCount(preTestLeaderboardRecords.Count - 1);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Leaderboard_Record_That_Does_Not_Exist_In_Accessor()
        {
            // Arrange
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();

            // Act
            var leaderboardRecords = await _helper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                leaderboardRecords.Should().NotBeNull();
                leaderboardRecords.Result.Should().Be(ModelResult.NotFound);
                leaderboardRecords.Data.Should().BeNull();

                // Check the database.
                var postTestLeaderboards = await _helper.GetLeaderboardRecords();
                postTestLeaderboards.Should().HaveCount(preTestLeaderboardRecords.Count);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var dbLeaderboardRecord = await _helper.AddJam();

            // Act
            Func<Task<IDataResponse<LeaderboardRecord?>>> func = () => _exceptionHelper.Accessor.DeleteAsync(dbLeaderboardRecord.Id);

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
