using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.GamesControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.GamesControllerTests.V1
{
    public class DeleteTests
    {
        private readonly GamesControllerHelper helper;

        public DeleteTests()
        {
            helper = new GamesControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Delete_Game_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.ControllerV1.Delete(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Relationship_Violation_Response_From_Delete_Game_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.RelationshipViolation);

            // Act
            var response = await helper.ControllerV1.Delete(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Delete_Game_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.ControllerV1.Delete(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Delete_Game_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.ControllerV1.Delete(Guid.NewGuid());

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
