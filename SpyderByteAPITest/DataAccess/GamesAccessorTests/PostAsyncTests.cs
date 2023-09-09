using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Games;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

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
            var game = await _helper.Accessor.PostAsync(postGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.Created);
                game.Data.Should().NotBeNull();
                game.Data.Should().BeEquivalentTo(postGame, options => options.Excluding(g => g.Image));
                game.Data!.Id.Should().NotBeEmpty();

                // Check the database.
                var dbGame = await _helper.GetGame(game.Data!.Id);
                dbGame.Should().NotBeNull();
                dbGame.Should().BeEquivalentTo(game.Data);
                dbGame.Should().BeEquivalentTo(postGame, options => options.Excluding(g => g.Image));

                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Duplicate_Game_To_Accessor()
        {
            // Arrange
            var preTestGames = await _helper.GetGames();
            var postGame = _helper.GeneratePostGame();

            // Act
            var response1 = await _helper.Accessor.PostAsync(postGame);
            var response2 = await _helper.Accessor.PostAsync(postGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the responses.
                response1.Should().NotBeNull();
                response1.Result.Should().Be(ModelResult.Created);
                response1.Data.Should().NotBeNull();

                response2.Should().NotBeNull();
                response2.Result.Should().Be(ModelResult.AlreadyExists);
                response2.Data.Should().NotBeNull();
                response2.Data.Should().BeEquivalentTo(response1.Data);

                // Check the database.
                var postTestGames = await _helper.GetGames();
                postTestGames.Should().HaveCount(preTestGames.Count + 1);
            }
        }

        [Fact]
        public async Task Can_Not_Add_Game_Without_Image_File_To_Accessor()
        {
            // Arrange
            var preTestGames = await _helper.GetGames();
            var postGame = _helper.GeneratePostGame();
            postGame.Image = null;

            // Act
            var game = await _helper.Accessor.PostAsync(postGame);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                game.Should().NotBeNull();
                game.Result.Should().Be(ModelResult.RequestDataIncomplete);
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
            var postGame = _helper.GeneratePostGame();

            // Act
            Func<Task<IDataResponse<Game?>>> func = () => _exceptionHelper.Accessor.PostAsync(postGame);

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
