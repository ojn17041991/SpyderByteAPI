using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SpyderByteResources.Enums;
using SpyderByteTest.API.DataControllerTests.Helpers;
using System.Net;

namespace SpyderByteTest.API.DataControllerTests
{
    public class BackupTests
    {
        private readonly DataControllerHelper helper;

        public BackupTests()
        {
            helper = new DataControllerHelper();
        }

        [Fact]
        public async Task Can_Receive_Ok_Response_From_Post_Backup_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.OK);

            // Act
            var response = await helper.Controller.Backup();

            // Assert
            response.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Can_Receive_Not_Found_Response_From_Post_Backup_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.NotFound);

            // Act
            var response = await helper.Controller.Backup();

            // Assert
            response.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Can_Receive_Error_Response_From_Post_Backup_Request()
        {
            // Arrange
            helper.SetCurrentModelResult(ModelResult.Error);

            // Act
            var response = await helper.Controller.Backup();

            // Assert
            var statusCodeResult = (StatusCodeResult)response;
            statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Can_Not_Make_Backup_Request_If_Feature_Flag_Is_Disabled()
        {
            // Arrange
            helper.SetAllowDatabaseBackups(false);

            // Act
            var response = await helper.Controller.Backup();

            // Assert
            response.Should().BeOfType<NotFoundResult>();

            // Cleanup
            helper.SetAllowDatabaseBackups(true);
        }
    }
}
