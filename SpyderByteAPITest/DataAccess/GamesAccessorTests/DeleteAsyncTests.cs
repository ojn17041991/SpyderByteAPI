using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class DeleteAsyncTests
    {
        private readonly GamesAccessorHelper _helper;

        public DeleteAsyncTests()
        {
            _helper = new GamesAccessorHelper();
        }

        // can delete game in accessor
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

        // can not delete game that does not exist in accessor
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
    }
}
