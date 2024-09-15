using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.UsersControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.UsersControllerTests
{
    public class PatchTests
    {
        private readonly UsersControllerHelper helper;

        public PatchTests()
        {
            helper = new UsersControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Patch_User_Request()
        {
            // Arrange
            var patchUser = helper.GeneratePatchUser();
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.Patch(patchUser);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Patch_User_Request()
        {
            // Arrange
            var patchUser = helper.GeneratePatchUser();
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.Patch(patchUser);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Request_Invalid_Response_From_Patch_User_Request()
        {
            // Arrange
            var patchUser = helper.GeneratePatchUser();
            helper.SetCurrentModelResult(ModelResult.RequestInvalid);

            // Act
            var response = await helper.Controller.Patch(patchUser);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Patch_User_Request()
        {
            // Arrange
            var patchUser = helper.GeneratePatchUser();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Patch(patchUser);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
