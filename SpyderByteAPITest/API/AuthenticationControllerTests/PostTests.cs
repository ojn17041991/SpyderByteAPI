using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.AuthenticationControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.AuthenticationControllerTests
{
    public class PostTests
    {
        private readonly AuthenticationControllerHelper helper;

        public PostTests()
        {
            helper = new AuthenticationControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Post_Authentication_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);
            var login = helper.GenerateLogin();

            // Act
            var response = await helper.Controller.Post(login);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Unauthorized_Response_From_Post_Authentication_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Unauthorized);
            var login = helper.GenerateLogin();

            // Act
            var response = await helper.Controller.Post(login);

            // Assert
            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Post_Authentication_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);
            var login = helper.GenerateLogin();

            // Act
            var response = await helper.Controller.Post(login);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
