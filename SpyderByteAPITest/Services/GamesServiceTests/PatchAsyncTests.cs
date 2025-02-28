using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.GamesServiceTests.Helpers;

namespace SpyderByteTest.Services.GamesServiceTests
{
    public class PatchAsyncTests
    {
        private readonly GamesServiceHelper _helper;

        public PatchAsyncTests()
        {
            _helper = new GamesServiceHelper();
        }

        [Fact]
        public async Task Can_Patch_Game_In_Service()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            var patchGame = _helper.GeneratePatchGame();
            patchGame.Id = storedGame.Id;

            // Act
            var returnedGame = await _helper.Service.PatchAsync(patchGame);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.OK);
            returnedGame.Data.Should().NotBeNull();
            returnedGame.Data!.Should().BeEquivalentTo(patchGame, options => options.Excluding(g => g.Image));

            returnedGame.Data!.Id.Should().Be(storedGame.Id);
            returnedGame.Data!.Name.Should().NotBe(storedGame.Name);
            returnedGame.Data!.Url.Should().NotBe(storedGame.Url);
            returnedGame.Data!.ImgurUrl.Should().NotBe(storedGame.ImgurUrl);
            returnedGame.Data!.ImgurImageId.Should().NotBe(storedGame.ImgurImageId);
            returnedGame.Data!.PublishDate.Should().NotBe(storedGame.PublishDate);
        }

        [Fact]
        public async Task Can_Not_Patch_Game_In_Service_If_Game_Does_Not_Exist()
        {
            // Arrange
            var patchGame = _helper.GeneratePatchGame();

            // Act
            var returnedGame = await _helper.Service.PatchAsync(patchGame);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.NotFound);
            returnedGame.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Not_Patch_Game_In_Service_If_Game_Name_Already_Exists()
        {
            // Arrange
            var storedGame1 = _helper.AddGame();
            var storedGame2 = _helper.AddGame();
            var patchGame = _helper.GeneratePatchGame();
            patchGame.Id = storedGame1.Id;
            patchGame.Name = storedGame2.Name;

            // Act
            var returnedGame = await _helper.Service.PatchAsync(patchGame);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.AlreadyExists);
            returnedGame.Data.Should().NotBeNull();
            returnedGame.Data!.Should().BeEquivalentTo(storedGame2);
        }
    }
}