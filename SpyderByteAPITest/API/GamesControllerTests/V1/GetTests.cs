using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.GamesControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.GamesControllerTests.V1
{
    public class GetTests
    {
        private readonly GamesControllerHelper helper;

        public GetTests()
        {
            helper = new GamesControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Get_Games_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);
            helper.SetAllowUseOfNonPaginatedEndpoints(true);

            // Act
            var response = await helper.ControllerV1.Get();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Implemented_Response_From_Get_Games_Request()
        {
            // Arrange
            helper.SetAllowUseOfNonPaginatedEndpoints(false);

            // Act
            var response = await helper.ControllerV1.Get();

            // Assert
            var objectResult = (ObjectResult)response;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotImplemented);
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Get_Games_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);
            helper.SetAllowUseOfNonPaginatedEndpoints(true);

            // Act
            var response = await helper.ControllerV1.Get();

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
