using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.LeaderboardsServiceTests.Helpers;

namespace SpyderByteTest.Services.LeaderboardsServiceTests
{
    public class PostRecordAsyncTests
    {
        private readonly LeaderboardsServiceHelper _helper;

        public PostRecordAsyncTests()
        {
            _helper = new LeaderboardsServiceHelper();
        }

        [Fact]
        public async Task Can_Post_Leaderboard_Record_In_Service()
        {
            // Arrange
            var storedLeaderboard = _helper.AddLeaderboard();
            var postLeaderboardRecord = _helper.GeneratePostLeaderboardRecord();
            postLeaderboardRecord.LeaderboardId = storedLeaderboard.Id;

            // Act
            var returnedLeaderboard = await _helper.Service.PostRecordAsync(postLeaderboardRecord);

            // Assert
            returnedLeaderboard.Should().NotBeNull();
            returnedLeaderboard.Result.Should().Be(ModelResult.OK);
            returnedLeaderboard.Data.Should().NotBeNull();
            returnedLeaderboard.Data!.Should().BeEquivalentTo(postLeaderboardRecord);
        }
    }
}
