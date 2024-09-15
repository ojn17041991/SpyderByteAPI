using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.LeaderboardsControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.LeaderboardsControllerTests
{
    public class DeleteRecordTests
    {
        private readonly LeaderboardsControllerHelper helper;

        public DeleteRecordTests()
        {
            helper = new LeaderboardsControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Delete_Record_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.DeleteRecord(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Delete_Record_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.DeleteRecord(Guid.NewGuid());

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Delete_Record_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.DeleteRecord(Guid.NewGuid());

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
