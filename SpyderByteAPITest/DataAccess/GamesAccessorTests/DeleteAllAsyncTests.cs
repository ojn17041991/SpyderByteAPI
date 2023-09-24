using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class DeleteAllAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public DeleteAllAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_All_Games_In_Accessor()
        {
            // Arrange
            var dbGame = await _helper.AddGame();

            // Act
            var games = await _helper.Accessor.DeleteAllAsync();

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                games.Should().NotBeNull();
                games.Result.Should().Be(ModelResult.OK);
                games.Data.Should().HaveCount(1);
                games.Data!.First().Should().BeEquivalentTo(dbGame);

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(0);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<IList<Game>?>>> func = () => _exceptionHelper.Accessor.DeleteAllAsync();

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
