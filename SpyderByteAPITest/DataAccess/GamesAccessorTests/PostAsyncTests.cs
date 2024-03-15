using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class PostAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public PostAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Add_Game_To_Accessor()
        {
            // Arrange
            var preTestGames = await _helper.GetGames();
            var postGame = _helper.GeneratePostGame();

            // Act
            var returnedGame = await _helper.Accessor.PostAsync(postGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.Created);
                returnedGame.Data.Should().NotBeNull();
                returnedGame.Data.Should().BeEquivalentTo(postGame);
                returnedGame.Data!.Id.Should().NotBeEmpty();

                // Check the database.
                var storedGame = await _helper.GetGame(returnedGame.Data!.Id);
                storedGame.Should().NotBeNull();
                storedGame.Should().BeEquivalentTo(returnedGame.Data);
                storedGame.Should().BeEquivalentTo(postGame);

                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count + 1);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var postGame = _helper.GeneratePostGame();

            // Act
            Func<Task<IDataResponse<Game?>>> func = () => _exceptionHelper.Accessor.PostAsync(postGame);

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
