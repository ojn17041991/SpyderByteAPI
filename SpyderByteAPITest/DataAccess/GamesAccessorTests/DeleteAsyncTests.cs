using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

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
            var dbGame = await _helper.AddGame();
            var preTestGames = await _helper.GetGames();

            // Act
            var game = await _helper.Accessor.DeleteAsync(dbGame.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.OK);
                game.Data.Should().BeEquivalentTo(dbGame);

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
            var game = await _helper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.NotFound);
                game.Data.Should().BeNull();

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var dbGame = await _helper.AddGame();

            // Act
            Func<Task<IDataResponse<Game?>>> func = () => _exceptionHelper.Accessor.DeleteAsync(dbGame.Id);

            // Assert
            using (new AssertionScope())
            {
                var games = await func.Invoke();
                games?.Should().NotBeNull();
                games?.Result.Should().Be(ModelResult.Error);
                games?.Data?.Should().BeNull();
            }
        }
    }
}
