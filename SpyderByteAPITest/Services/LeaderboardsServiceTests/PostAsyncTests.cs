using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.LeaderboardsServiceTests.Helpers;

namespace SpyderByteTest.Services.LeaderboardsServiceTests
{
    public class PostAsyncTests
    {
        private readonly LeaderboardsServiceHelper _helper;

        public PostAsyncTests()
        {
            _helper = new LeaderboardsServiceHelper();
        }

        [Fact]
        public async Task Can_Post_Leaderboard_In_Service()
        {
            // Arrange
            var postLeaderboard = _helper.GeneratePostLeaderboard();

            // Act
            var returnedLeaderboard = await _helper.Service.PostAsync(postLeaderboard);

            // Assert
            returnedLeaderboard.Should().NotBeNull();
            returnedLeaderboard.Result.Should().Be(ModelResult.OK);
            returnedLeaderboard.Data.Should().NotBeNull();
            returnedLeaderboard.Data!.LeaderboardGame.GameId.Should().Be(postLeaderboard.GameId);
            returnedLeaderboard.Data!.LeaderboardGame.Game.Id.Should().Be(postLeaderboard.GameId);
        }
    }
}
