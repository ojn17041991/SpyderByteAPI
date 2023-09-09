using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class GetAllAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public GetAllAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Games_From_Accessor()
        {
            // Arrange
            var game = await _helper.AddGame();

            // Act
            var games = await _helper.Accessor.GetAllAsync();

            // Assert
            using (new AssertionScope())
            {
                games.Should().NotBeNull();
                games.Result.Should().Be(ModelResult.OK);
                games.Data.Should().NotBeNull();
                games.Data.Should().HaveCount(1);
                games.Data.Should().ContainEquivalentOf(game);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<IList<Game>?>>> func = () => _exceptionHelper.Accessor.GetAllAsync();

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