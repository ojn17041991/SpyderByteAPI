using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.GamesServiceTests.Helpers;

namespace SpyderByteTest.Services.GamesServiceTests
{
    public class PostAsyncTests
    {
        private readonly GamesServiceHelper _helper;

        public PostAsyncTests()
        {
            _helper = new GamesServiceHelper();
        }

        [Fact]
        public async Task Can_Post_Game_In_Service()
        {
            // Arrange
            var postGame = _helper.GeneratePostGame();

            // Act
            var returnedGame = await _helper.Service.PostAsync(postGame);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.Created);
            returnedGame.Data.Should().NotBeNull();
            returnedGame.Data!.Should().BeEquivalentTo(postGame, options => options.Excluding(g => g.Image));
        }

        [Fact]
        public async Task Can_Not_Post_Game_In_Service_If_Image_Is_Not_Included()
        {
            // Arrange
            var postGame = _helper.GeneratePostGame();
            postGame.Image = null!;

            // Act
            var returnedGame = await _helper.Service.PostAsync(postGame);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.RequestDataIncomplete);
            returnedGame.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Not_Post_Game_In_Service_If_Game_Name_Already_Exists()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            var postGame = _helper.GeneratePostGame();
            postGame.Name = storedGame.Name;

            // Act
            var returnedGame = await _helper.Service.PostAsync(postGame);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.AlreadyExists);
            returnedGame.Data.Should().NotBeNull();
            returnedGame.Data!.Should().BeEquivalentTo(storedGame);
        }
    }
}
