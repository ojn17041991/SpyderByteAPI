using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests
{
    public class DeleteRecordAsyncTests
    {
        private readonly LeaderboardsAccessorHelper _helper;
        private readonly LeaderboardsAccessorExceptionHelper _exceptionHelper;

        public DeleteRecordAsyncTests()
        {
            _helper = new LeaderboardsAccessorHelper();
            _exceptionHelper = new LeaderboardsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_Leaderboard_Record_In_Accessor()
        {
            // Arrange
            var storedLeaderboard = await _helper.AddLeaderboardWithRecords(3);
            var storedLeaderboardRecord = storedLeaderboard.LeaderboardRecords.First()!;
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();

            // Act
            var returnedLeaderboardRecord = await _helper.Accessor.DeleteRecordAsync(storedLeaderboardRecord.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedLeaderboardRecord.Should().NotBeNull();
                returnedLeaderboardRecord.Result.Should().Be(ModelResult.OK);
                returnedLeaderboardRecord.Data.Should().NotBeNull();
                returnedLeaderboardRecord.Data!.Id.Should().Be(storedLeaderboardRecord.Id);

                // Check the database.
                var postTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
                postTestLeaderboardRecords.Should().HaveCount(preTestLeaderboardRecords.Count - 1);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Leaderboard_Record_In_Accessor_That_Does_Not_Exist()
        {
            // Arrange
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();

            // Act
            var leaderboardRecords = await _helper.Accessor.DeleteRecordAsync(Guid.NewGuid());

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
            var storedLeaderboard = await _helper.AddLeaderboardWithRecords(3);
            var storedLeaderboardRecord = storedLeaderboard.LeaderboardGame!.Leaderboard.LeaderboardRecords.First();

            // Act
            Func<Task<IDataResponse<LeaderboardRecord?>>> func = () => _exceptionHelper.Accessor.DeleteRecordAsync(storedLeaderboardRecord.Id);

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
