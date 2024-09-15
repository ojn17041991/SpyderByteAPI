using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.LeaderboardsControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.LeaderboardsControllerTests
{
    public class PostRecordTests
    {
        private readonly LeaderboardsControllerHelper helper;

        public PostRecordTests()
        {
            helper = new LeaderboardsControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Created_Response_From_Post_Record_Request()
        {
            // Arrange
            var postRecord = helper.GeneratePostRecord();
            helper.SetCurrentModelResult(ModelResult.Created);

            // Act
            var response = await helper.Controller.PostRecord(postRecord);

            // Assert
            response.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Post_Record_Request()
        {
            // Arrange
            var postRecord = helper.GeneratePostRecord();
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.PostRecord(postRecord);

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Post_Record_Request()
        {
            // Arrange
            var postRecord = helper.GeneratePostRecord();
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.PostRecord(postRecord);

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Can_Receive_Unauthorized_Response_From_Post_Record_Request()
        {
            // Arrange
            var postRecord = helper.GeneratePostRecord();
            helper.SetAuthorizeRequests(false);

            // Act
            var response = await helper.Controller.PostRecord(postRecord);

            // Assert
            response.Should().BeOfType<UnauthorizedResult>();

            // Cleanup
            helper.SetAuthorizeRequests(true);
        }
    }
}
