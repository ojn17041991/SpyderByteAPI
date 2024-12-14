using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.GamesServiceTests.Helpers;

namespace SpyderByteTest.Services.GamesServiceTests
{
    public class GetAllAsyncTests
    {
        private readonly GamesServiceHelper _helper;

        public GetAllAsyncTests()
        {
            _helper = new GamesServiceHelper();
        }

        [Fact]
        public async Task Can_Get_Games_From_Service()
        {
            // Arrange
            var storedGame = _helper.AddGame();

            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 10;
            string? order = null;
            string? direction = null;

            // Act
            var returnedGames = await _helper.Service.GetAllAsync(name, type, page, pageSize, order, direction);

            // Assert
            returnedGames.Should().NotBeNull();
            returnedGames.Result.Should().Be(ModelResult.OK);
            returnedGames.Data.Should().NotBeNull();
            returnedGames.Data!.Items.Count().Should().Be(1);
            returnedGames.Data!.Items.Should().ContainEquivalentOf(storedGame);
        }
    }
}
