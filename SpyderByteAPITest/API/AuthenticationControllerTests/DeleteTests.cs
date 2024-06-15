using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.AuthenticationControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.AuthenticationControllerTests
{
    public class DeleteTests
    {
        private readonly AuthenticationControllerHelper helper;

        public DeleteTests()
        {
            helper = new AuthenticationControllerHelper();
        }

        [Fact]
        public void Can_Receive_Ok_Response_From_Delete_Authentication_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = helper.Controller.Delete();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Can_Receive_Error_Response_From_Delete_Authentication_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = helper.Controller.Delete();

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
