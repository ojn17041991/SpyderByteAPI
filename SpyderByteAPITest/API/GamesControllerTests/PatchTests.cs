using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.GamesControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.GamesControllerTests
{
    public class PatchTests
    {
        private readonly GamesControllerHelper helper;

        public PatchTests()
        {
            helper = new GamesControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Patch_Game_Request()
        {
            // Arrange
            var patchGame = helper.GeneratePatchGame();
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.Patch(patchGame);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Already_Exists_Response_From_Patch_Game_Request()
        {
            // Arrange
            var patchGame = helper.GeneratePatchGame();
            helper.SetCurrentModelResult(ModelResult.AlreadyExists);

            // Act
            var response = await helper.Controller.Patch(patchGame);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Patch_Game_Request()
        {
            // Arrange
            var patchGame = helper.GeneratePatchGame();
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.Patch(patchGame);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Patch_Game_Request()
        {
            // Arrange
            var patchGame = helper.GeneratePatchGame();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Patch(patchGame);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
