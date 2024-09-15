using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.LeaderboardsControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.LeaderboardsControllerTests
{
    public class PostLeaderboardTests
    {
        private readonly LeaderboardsControllerHelper helper;

        public PostLeaderboardTests()
        {
            helper = new LeaderboardsControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Created_Response_From_Post_Leaderboard_Request()
        {
            // Arrange
            var postLeaderboard = helper.GeneratePostLeaderboard();
            helper.SetCurrentModelResult(ModelResult.Created);

            // Act
            var response = await helper.Controller.PostLeaderboard(postLeaderboard);

            // Assert
            response.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Post_Leaderboard_Request()
        {
            // Arrange
            var postLeaderboard = helper.GeneratePostLeaderboard();
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.PostLeaderboard(postLeaderboard);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Already_Exists_Response_From_Post_Leaderboard_Request()
        {
            // Arrange
            var postLeaderboard = helper.GeneratePostLeaderboard();
            helper.SetCurrentModelResult(ModelResult.AlreadyExists);

            // Act
            var response = await helper.Controller.PostLeaderboard(postLeaderboard);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Post_Leaderboard_Request()
        {
            // Arrange
            var postLeaderboard = helper.GeneratePostLeaderboard();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.PostLeaderboard(postLeaderboard);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
