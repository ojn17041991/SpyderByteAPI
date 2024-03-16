using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.LeaderboardsServiceTests.Helpers;

namespace SpyderByteTest.Services.LeaderboardsServiceTests
{
    public class GetAsyncTests
    {
        private readonly LeaderboardsServiceHelper _helper;

        public GetAsyncTests()
        {
            _helper = new LeaderboardsServiceHelper();
        }

        [Fact]
        public async Task Can_Get_Leaderboard_From_Service()
        {
            // Arrange
            var storedLeaderboard = _helper.AddLeaderboard();

            // Act
            var returnedLeaderboard = await _helper.Service.GetAsync(storedLeaderboard.Id);

            // Assert
            returnedLeaderboard.Should().NotBeNull();
            returnedLeaderboard.Result.Should().Be(ModelResult.OK);
            returnedLeaderboard.Data.Should().NotBeNull();
            returnedLeaderboard.Data!.Should().BeEquivalentTo(storedLeaderboard);
        }
    }
}
