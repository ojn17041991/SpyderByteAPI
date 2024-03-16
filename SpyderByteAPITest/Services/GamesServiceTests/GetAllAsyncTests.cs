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

            // Act
            var returnedGames = await _helper.Service.GetAllAsync();

            // Assert
            returnedGames.Should().NotBeNull();
            returnedGames.Result.Should().Be(ModelResult.OK);
            returnedGames.Data.Should().NotBeNull();
            returnedGames.Data!.Count().Should().Be(1);
            returnedGames.Data!.Should().ContainEquivalentOf(storedGame);
        }
    }
}
