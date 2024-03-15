using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;
using SpyderByteDataAccess.Models.Games;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class PatchAsyncTests
    {
        private readonly GamesAccessorHelper _helper;
        private readonly GamesAccessorExceptionHelper _exceptionHelper;

        public PatchAsyncTests()
        {
            _helper = new GamesAccessorHelper();
            _exceptionHelper = new GamesAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Patch_Game_In_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGame();
            var patchGame = _helper.GeneratePatchGame();
            patchGame.Id = storedGame.Id;

            // Act
            var returnedGame = await _helper.Accessor.PatchAsync(patchGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedGame.Should().NotBeNull();
                returnedGame.Result.Should().Be(ModelResult.OK);
                returnedGame.Data.Should().NotBeNull();
                returnedGame.Data.Should().BeEquivalentTo(patchGame);

                // Check the database.
                var patchedStoredGame = await _helper.GetGame(patchGame.Id);
                patchedStoredGame.Should().NotBeNull();
                patchedStoredGame.Should().BeEquivalentTo(returnedGame.Data);
                patchedStoredGame!.Id.Should().Be(storedGame.Id);
                patchedStoredGame!.Name.Should().NotBe(storedGame.Name);
                patchedStoredGame!.HtmlUrl.Should().NotBe(storedGame.HtmlUrl);
                patchedStoredGame!.ImgurUrl.Should().NotBe(storedGame.ImgurUrl);
                patchedStoredGame!.ImgurImageId.Should().NotBe(storedGame.ImgurImageId);
                patchedStoredGame!.PublishDate.Should().NotBe(storedGame.PublishDate);
            }
        }

        [Fact]
        public async Task Can_Not_Patch_Game_That_Does_Not_Exist_In_Accessor()
        {
            // Arrange
            var patchGame = _helper.GeneratePatchGame();

            // Act
            var game = await _helper.Accessor.PatchAsync(patchGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.NotFound);
                game.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var patchGame = _helper.GeneratePatchGame();

            // Act
            Func<Task<IDataResponse<Game?>>> func = () => _exceptionHelper.Accessor.PatchAsync(patchGame);

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
