using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.AuthenticationControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.AuthenticationControllerTests
{
    public class RefreshTests
    {
        private readonly AuthenticationControllerHelper helper;

        public RefreshTests()
        {
            helper = new AuthenticationControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Refresh_Authentication_Request()
        {
            // Arrange
            helper.SetAllowAuthenticationRefresh(true);
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.Refresh();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Unauthorized_Response_From_Refresh_Authentication_Request()
        {
            // Arrange
            helper.SetAllowAuthenticationRefresh(true);
            helper.SetCurrentModelResult(ModelResult.Unauthorized);

            // Act
            var response = await helper.Controller.Refresh();

            // Assert
            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Refresh_Authentication_Request()
        {
            // Arrange
            helper.SetAllowAuthenticationRefresh(true);
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Refresh();

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Can_Not_Make_Refresh_Authentication_Request_If_Feature_Flag_Is_Disabled()
        {
            // Arrange
            helper.SetAllowAuthenticationRefresh(false);

            // Act
            var response = await helper.Controller.Refresh();

            // Assert
            response.Should().BeOfType<NotFoundResult>();
        }
    }
}
