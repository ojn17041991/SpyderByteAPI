using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.LeaderboardsServiceTests.Helpers;

namespace SpyderByteTest.Services.LeaderboardsServiceTests
{
    public class DeleteAsyncTests
    {
        private readonly LeaderboardsServiceHelper _helper;

        public DeleteAsyncTests()
        {
            _helper = new LeaderboardsServiceHelper();
        }

        [Fact]
        public async Task Can_Delete_Leaderboard_In_Service()
        {
            // Arrange
            var storedLeaderboard = _helper.AddLeaderboard();

            // Act
            var returnedLeaderboard = await _helper.Service.DeleteAsync(storedLeaderboard.Id);

            // Assert
            returnedLeaderboard.Should().NotBeNull();
            returnedLeaderboard.Result.Should().Be(ModelResult.OK);
            returnedLeaderboard.Data.Should().NotBeNull();
            returnedLeaderboard.Data!.Should().BeEquivalentTo(storedLeaderboard);
        }
    }
}
