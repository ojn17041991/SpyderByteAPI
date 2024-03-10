using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.DataAccess.LeaderboardAccessorTests.Helper;
using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteAPITest.DataAccess.LeaderboardAccessorTests
{
    public class PostAsyncTests
    {
        private readonly LeaderboardAccessorHelper _helper;
        private readonly LeaderboardAccessorExceptionHelper _exceptionHelper;

        public PostAsyncTests()
        {
            _helper = new LeaderboardAccessorHelper();
            _exceptionHelper = new LeaderboardAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Add_Leaderboard_Record_For_Game_To_Accessor()
        {
            // Arrange
            var game = await _helper.AddGame();
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
            var postLeaderboardRecord = _helper.GeneratePostLeaderboardRecord();

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
                var dbLeaderboardRecord = await _helper.GetLeaderboardRecord(leaderboardRecord.Data!.Id);
                dbLeaderboardRecord.Should().NotBeNull();
                dbLeaderboardRecord.Should().BeEquivalentTo(leaderboardRecord.Data);
                dbLeaderboardRecord.Should().BeEquivalentTo(postLeaderboardRecord);

                var postTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
                postTestLeaderboardRecords.Should().HaveCount(preTestLeaderboardRecords.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Add_Leaderboard_Record_For_Jam_To_Accessor()
        {
            // Arrange
            var game = await _helper.AddGame();
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
            var postLeaderboardRecord = _helper.GeneratePostLeaderboardRecord();

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
                var dbLeaderboardRecord = await _helper.GetLeaderboardRecord(leaderboardRecord.Data!.Id);
                dbLeaderboardRecord.Should().NotBeNull();
                dbLeaderboardRecord.Should().BeEquivalentTo(leaderboardRecord.Data);
                dbLeaderboardRecord.Should().BeEquivalentTo(postLeaderboardRecord);

                var postTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
                postTestLeaderboardRecords.Should().HaveCount(preTestLeaderboardRecords.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Leaderboard_Record_With_Invalid_Game_Id_To_Accessor()
        {
            // Arrange
            var preTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
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

                // Check the database.
                var postTestLeaderboardRecords = await _helper.GetLeaderboardRecords();
                postTestLeaderboardRecords.Should().HaveCount(preTestLeaderboardRecords.Count);
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
