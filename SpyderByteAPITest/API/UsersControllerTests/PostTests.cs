using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.UsersControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.UsersControllerTests
{
    public class PostTests
    {
        private readonly UsersControllerHelper helper;

        public PostTests()
        {
            helper = new UsersControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Created_Response_From_Post_User_Request()
        {
            // Arrange
            var postUser = helper.GeneratePostUser();
            helper.SetCurrentModelResult(ModelResult.Created);

            // Act
            var response = await helper.Controller.Post(postUser);

            // Assert
            response.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Can_Receive_Already_Exists_Response_From_Post_User_Request()
        {
            // Arrange
            var postUser = helper.GeneratePostUser();
            helper.SetCurrentModelResult(ModelResult.AlreadyExists);

            // Act
            var response = await helper.Controller.Post(postUser);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Post_User_Request()
        {
            // Arrange
            var postUser = helper.GeneratePostUser();
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.Post(postUser);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Request_Invalid_Response_From_Post_User_Request()
        {
            // Arrange
            var postUser = helper.GeneratePostUser();
            helper.SetCurrentModelResult(ModelResult.RequestInvalid);

            // Act
            var response = await helper.Controller.Post(postUser);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Post_User_Request()
        {
            // Arrange
            var postUser = helper.GeneratePostUser();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Post(postUser);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
