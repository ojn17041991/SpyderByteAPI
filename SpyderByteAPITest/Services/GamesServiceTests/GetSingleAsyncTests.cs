using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.GamesServiceTests.Helpers;

namespace SpyderByteTest.Services.GamesServiceTests
{
    public class GetSingleAsyncTests
    {
        private readonly GamesServiceHelper _helper;

        public GetSingleAsyncTests()
        {
            _helper = new GamesServiceHelper();
        }

        [Fact]
        public async Task Can_Get_Game_From_Service()
        {
            // Arrange
            var storedGame = _helper.AddGame();

            // Act
            var returnedGame = await _helper.Service.GetSingleAsync(storedGame.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.OK);
            returnedGame.Data.Should().NotBeNull();
            returnedGame.Data!.Should().BeEquivalentTo(storedGame);
        }
    }
}