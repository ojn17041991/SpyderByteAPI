using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.UsersControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.UsersControllerTests
{
    public class GetTests
    {
        private readonly UsersControllerHelper helper;

        public GetTests()
        {
            helper = new UsersControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Get_User_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.Get(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Get_User_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.Get(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Get_User_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Get(Guid.NewGuid());

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
