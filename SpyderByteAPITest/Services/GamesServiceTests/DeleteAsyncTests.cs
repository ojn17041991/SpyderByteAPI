using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.GamesServiceTests.Helper;

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
        public async Task Can_Not_Delete_Game_From_Service_If_User_Is_Dependent_On_Game()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            _helper.RemoveGameLeaderboardRelationship(storedGame.Id);

            // Act
            var returnedGame = await _helper.Service.DeleteAsync(storedGame.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.RelationshipViolation);
            returnedGame.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Not_Delete_Game_From_Service_If_Leaderboard_Is_Dependent_On_Game()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            _helper.RemoveGameUserRelationship(storedGame.Id);

            // Act
            var returnedGame = await _helper.Service.DeleteAsync(storedGame.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.RelationshipViolation);
            returnedGame.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Not_Delete_Game_From_Service_If_Game_Does_Not_Exist()
        {
            // Arrange

            // Act
            var returnedGame = await _helper.Service.DeleteAsync(Guid.NewGuid());

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.NotFound);
            returnedGame.Data.Should().BeNull();
        }
    }
}
