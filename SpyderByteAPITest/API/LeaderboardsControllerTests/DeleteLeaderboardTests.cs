using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.LeaderboardsControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.LeaderboardsControllerTests
{
    public class DeleteLeaderboardTests
    {
        private readonly LeaderboardsControllerHelper helper;

        public DeleteLeaderboardTests()
        {
            helper = new LeaderboardsControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Delete_Leaderboard_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.DeleteLeaderboard(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Delete_Leaderboard_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.DeleteLeaderboard(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Delete_Leaderboard_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.DeleteLeaderboard(Guid.NewGuid());

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
