using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class DeleteAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public DeleteAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_Game_In_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGame();
            await _helper.RemoveUserGameRelationship(storedGame.Id);
            await _helper.RemoveLeaderboardGameRelationship(storedGame.Id);
            var preTestGames = await _helper.GetGames();

            // Act
            var returnedGame = await _helper.Accessor.DeleteAsync(storedGame.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.OK);
                returnedGame.Data.Should().BeEquivalentTo(storedGame, options =>
                    options.Excluding(g => g.LeaderboardGame)
                        .Excluding(g => g.UserGame));

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count - 1);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Game_That_Does_Not_Exist_In_Accessor()
        {
            // Arrange
            var preTestGames = await _helper.GetGames();

            // Act
            var returnedGame = await _helper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.NotFound);
                returnedGame.Data.Should().BeNull();

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Game_When_User_Is_Dependent_On_Game()
        {
            // Arrange
            var storedGame = await _helper.AddGame();
            await _helper.RemoveLeaderboardGameRelationship(storedGame.Id);
            var preTestGames = await _helper.GetGames();

            // Act
            var returnedGame = await _helper.Accessor.DeleteAsync(storedGame.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.RelationshipViolation);
                returnedGame.Data.Should().NotBeNull();
                returnedGame.Data.Should().BeEquivalentTo(storedGame, options =>
                    options.Excluding(g => g.LeaderboardGame)
                        .Excluding(g => g.UserGame));

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Game_When_Leaderboard_Is_Dependent_On_Game()
        {
            // Arrange
            var storedGame = await _helper.AddGame();
            await _helper.RemoveUserGameRelationship(storedGame.Id);
            var preTestGames = await _helper.GetGames();

            // Act
            var returnedGame = await _helper.Accessor.DeleteAsync(storedGame.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.RelationshipViolation);
                returnedGame.Data.Should().NotBeNull();
                returnedGame.Data.Should().BeEquivalentTo(storedGame, options =>
                    options.Excluding(g => g.LeaderboardGame)
                        .Excluding(g => g.UserGame));

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var storedGame = await _helper.AddGame();

            // Act
            Func<Task<IDataResponse<Game?>>> func = () => _exceptionHelper.Accessor.DeleteAsync(storedGame.Id);

            // Assert
            using (new AssertionScope())
            {
                var returnedGame = await func.Invoke();
                returnedGame?.Should().NotBeNull();
                returnedGame?.Result.Should().Be(ModelResult.Error);
                returnedGame?.Data?.Should().BeNull();
            }
        }
    }
}
