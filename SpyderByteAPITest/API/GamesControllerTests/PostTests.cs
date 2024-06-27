using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.GamesControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.GamesControllerTests
{
    public class PostTests
    {
        private readonly GamesControllerHelper helper;

        public PostTests()
        {
            helper = new GamesControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Created_Response_From_Post_Game_Request()
        {
            // Arrange
            var postGame = helper.GeneratePostGame();
            helper.SetCurrentModelResult(ModelResult.Created);

            // Act
            var response = await helper.Controller.Post(postGame);

            // Assert
            response.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Can_Receive_Request_Data_Incomplete_Response_From_Post_Game_Request()
        {
            // Arrange
            var postGame = helper.GeneratePostGame();
            helper.SetCurrentModelResult(ModelResult.RequestDataIncomplete);

            // Act
            var response = await helper.Controller.Post(postGame);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Already_Exists_Response_From_Post_Game_Request()
        {
            // Arrange
            var postGame = helper.GeneratePostGame();
            helper.SetCurrentModelResult(ModelResult.AlreadyExists);

            // Act
            var response = await helper.Controller.Post(postGame);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Post_Game_Request()
        {
            // Arrange
            var postGame = helper.GeneratePostGame();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Post(postGame);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
