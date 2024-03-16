using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.LeaderboardsServiceTests.Helpers;

namespace SpyderByteTest.Services.LeaderboardsServiceTests
{
    public class PatchAsyncTests
    {
        private readonly LeaderboardsServiceHelper _helper;

        public PatchAsyncTests()
        {
            _helper = new LeaderboardsServiceHelper();
        }

        [Fact]
        public async Task Can_Patch_Leaderboard_In_Service()
        {
            // Arrange
            var storedLeaderboard = _helper.AddLeaderboard();
            var patchLeaderboard = _helper.GeneratePatchLeaderboard();
            patchLeaderboard.Id = storedLeaderboard.Id;

            // Act
            var returnedLeaderboard = await _helper.Service.PatchAsync(patchLeaderboard);

            // Assert
            returnedLeaderboard.Should().NotBeNull();
            returnedLeaderboard.Result.Should().Be(ModelResult.OK);
            returnedLeaderboard.Data.Should().NotBeNull();
            returnedLeaderboard.Data!.Should().BeEquivalentTo(storedLeaderboard);
        }
    }
}
