using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteTest.DataAccess.LeaderboardsAccessorTests.Helpers;

namespace SpyderByteTest.DataAccess.LeaderboardsAccessorTests
{
    public class PostRecordAsyncTests
    {
        private readonly LeaderboardsAccessorHelper _helper;
        private readonly LeaderboardsAccessorExceptionHelper _exceptionHelper;

        public PostRecordAsyncTests()
        {
            _helper = new LeaderboardsAccessorHelper();
            _exceptionHelper = new LeaderboardsAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Add_Leaderboard_Record_To_Accessor()
        {
            // Arrange
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
            var storedLeaderboard = await _helper.AddLeaderboardWithoutRecords();
            var postLeaderboardRecord = _helper.GeneratePostLeaderboardRecord();
            postLeaderboardRecord.LeaderboardId = storedLeaderboard.Id;

            // Act
            var leaderboardRecord = await _helper.Accessor.PostRecordAsync(postLeaderboardRecord);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                leaderboardRecord.Should().NotBeNull();
                leaderboardRecord.Result.Should().Be(ModelResult.Created);
                leaderboardRecord.Data.Should().NotBeNull();
                leaderboardRecord.Data.Should().BeEquivalentTo(postLeaderboardRecord);
                leaderboardRecord.Data!.Id.Should().NotBeEmpty();

                // Check the database.
                var updatedStoredLeaderboardRecord = await _helper.GetLeaderboardRecord(leaderboardRecord.Data!.Id);
                updatedStoredLeaderboardRecord.Should().NotBeNull();
                updatedStoredLeaderboardRecord.Should().BeEquivalentTo(leaderboardRecord.Data);
                updatedStoredLeaderboardRecord.Should().BeEquivalentTo(postLeaderboardRecord);

                var postTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
                postTestLeaderboardRecords.Should().HaveCount(preTestLeaderboardRecords.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Leaderboard_Record_To_Accessor_When_Leaderboard_Does_Not_Exist()
        {
            // Arrange
            var postLeaderboardRecord = _helper.GeneratePostLeaderboardRecord();

            // Act
            var leaderboardRecord = await _helper.Accessor.PostRecordAsync(postLeaderboardRecord);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                leaderboardRecord.Should().NotBeNull();
                leaderboardRecord.Result.Should().Be(ModelResult.NotFound);
                leaderboardRecord.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var postLeaderboardRecord = _helper.GeneratePostLeaderboardRecord();

            // Act
            Func<Task<IDataResponse<LeaderboardRecord?>>> func = () => _exceptionHelper.Accessor.PostRecordAsync(postLeaderboardRecord);

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
