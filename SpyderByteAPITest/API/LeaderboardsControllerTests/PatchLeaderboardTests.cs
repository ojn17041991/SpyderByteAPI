﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.LeaderboardsControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.LeaderboardsControllerTests
{
    public class PatchLeaderboardTests
    {
        private readonly LeaderboardsControllerHelper helper;

        public PatchLeaderboardTests()
        {
            helper = new LeaderboardsControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Patch_Leaderboard_Request()
        {
            // Arrange
            var patchLeaderboard = helper.GeneratePatchLeaderboard();
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.PatchLeaderboard(patchLeaderboard);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Patch_Leaderboard_Request()
        {
            // Arrange
            var patchLeaderboard = helper.GeneratePatchLeaderboard();
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.PatchLeaderboard(patchLeaderboard);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Already_Exists_Response_From_Patch_Leaderboard_Request()
        {
            // Arrange
            var patchLeaderboard = helper.GeneratePatchLeaderboard();
            helper.SetCurrentModelResult(ModelResult.AlreadyExists);

            // Act
            var response = await helper.Controller.PatchLeaderboard(patchLeaderboard);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Patch_Leaderboard_Request()
        {
            // Arrange
            var patchLeaderboard = helper.GeneratePatchLeaderboard();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.PatchLeaderboard(patchLeaderboard);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
