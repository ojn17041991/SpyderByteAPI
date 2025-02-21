using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.GamesControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.GamesControllerTests
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

            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 10;
            string? order = null;
            string? direction = null;

            // Act
            var response = await helper.Controller.Get(name, type, page, pageSize, order, direction);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Get_Games_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            string? name = null;
            GameType? type = null;
            int page = 1;
            int pageSize = 10;
            string? order = null;
            string? direction = null;

            // Act
            var response = await helper.Controller.Get(name, type, page, pageSize, order, direction);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
