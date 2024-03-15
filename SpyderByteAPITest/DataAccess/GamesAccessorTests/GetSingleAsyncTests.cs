using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class GetSingleAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public GetSingleAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_Single_Game_From_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGame();

            // Act
            var returnedGame = await _helper.Accessor.GetSingleAsync(storedGame.Id);

            // Assert
            using (new AssertionScope())
            {
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.OK);
                returnedGame.Data.Should().NotBeNull();
                returnedGame.Data.Should().BeEquivalentTo(storedGame);
            }
        }

        [Fact]
        public async Task Can_Not_Get_Single_Game_From_Accessor_With_Invalid_Id()
        {
            // Arrange
            await _helper.AddGame();

            // Act
            var returnedGame = await _helper.Accessor.GetSingleAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.NotFound);
                returnedGame.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var storedGame = await _helper.AddGame();

            // Act
            Func<Task<IDataResponse<Game?>>> func = () => _exceptionHelper.Accessor.GetSingleAsync(storedGame.Id);

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
