using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.GamesServiceTests.Helpers;

namespace SpyderByteTest.Services.GamesServiceTests
{
    public class DeleteAsyncTests
    {
        private readonly GamesServiceHelper _helper;

        public DeleteAsyncTests()
        {
            _helper = new GamesServiceHelper();
        }

        [Fact]
        public async Task Can_Delete_Game_From_Service()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            _helper.RemoveGameUserRelationship(storedGame.Id);
            _helper.RemoveGameLeaderboardRelationship(storedGame.Id);

            // Act
            var returnedGame = await _helper.Service.DeleteAsync(storedGame.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.OK);
            returnedGame.Data.Should().NotBeNull();
            returnedGame.Data!.Should().BeEquivalentTo(storedGame, options =>
                options.Excluding(g => g.LeaderboardGame)
                    .Excluding(g => g.UserGame));
        }

        [Fact]
        public async Task Can_Delete_Game_From_Service_With_Error_If_Imgur_Delete_Fails()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            _helper.RemoveGameUserRelationship(storedGame.Id);
            _helper.RemoveGameLeaderboardRelationship(storedGame.Id);
            _helper.IncludeBadImgurId(storedGame.Id);

            // Act
            var returnedGame = await _helper.Service.DeleteAsync(storedGame.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.Error);
            returnedGame.Data.Should().BeNull();
        }
    }
}
