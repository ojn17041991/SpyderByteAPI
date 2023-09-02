using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.DataAccess.GamesAccessorTests.Helper;

namespace SpyderByteAPITest.DataAccess.GamesAccessorTests
{
    public class DeleteAllAsyncTests
    {
        private readonly GamesAccessorHelper _helper;

        public DeleteAllAsyncTests()
        {
            _helper = new GamesAccessorHelper();
        }

        // can delete all games in accessor
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
    }
}
