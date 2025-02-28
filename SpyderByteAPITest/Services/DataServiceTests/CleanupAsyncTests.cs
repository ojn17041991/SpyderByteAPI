using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteServices.Models.Data;
using SpyderByteTest.Services.DataServiceTests.Enums;
using SpyderByteTest.Services.DataServiceTests.Helpers;

namespace SpyderByteTest.Services.DataServiceTests
{
    public class CleanupAsyncTests
    {
        private readonly DataServiceHelper _helper;

        public CleanupAsyncTests()
        {
            _helper = new DataServiceHelper();
        }

        [Fact]
        public async Task Can_Cleanup_Data()
        {
            // Arrange
            int retentionPeriod = 1;
            _helper.SetRetentionPeriod(retentionPeriod);

            StorageFile file = _helper.GenerateStorageFile();
            file.CreatedDate = DateTime.UtcNow.AddHours(-retentionPeriod * 2);
            _helper.AddFileToStorage(file);

            // Act
            var response = await _helper.Service.CleanupAsync();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.OK);
            response.Data.Should().BeTrue();

            var files = _helper.GetStorageFiles();
            files.Should().HaveCount(0);
        }

        [Fact]
        public async Task Only_Files_Outside_Of_Retention_Period_Are_Deleted()
        {
            // Arrange
            int retentionPeriod = 1;
            _helper.SetRetentionPeriod(retentionPeriod);

            StorageFile file = _helper.GenerateStorageFile();
            file.CreatedDate = DateTime.UtcNow.AddHours(-retentionPeriod * 0.5);
            _helper.AddFileToStorage(file);

            // Act
            var response = await _helper.Service.CleanupAsync();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.OK);
            response.Data.Should().BeTrue();

            var files = _helper.GetStorageFiles();
            files.Should().HaveCount(1);
        }

        [Fact]
        public async Task Can_Not_Cleanup_Data_When_GetFilesAsync_Request_Fails()
        {
            // Arrange
            _helper.BuildFullConfiguration();
            _helper.SetStorageServiceResponse(DataFunction.GetFilesAsync, false);

            // Act
            var response = await _helper.Service.CleanupAsync();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.Error);
            response.Data.Should().BeFalse();

            // Teardown
            _helper.SetStorageServiceResponse(DataFunction.GetFilesAsync, true);
        }

        [Fact]
        public async Task Cleanup_Is_Not_Blocked_When_DeleteAsync_Request_Fails()
        {
            // Arrange
            int retentionPeriod = 1;
            _helper.SetRetentionPeriod(retentionPeriod);

            StorageFile file = _helper.GenerateStorageFile();
            file.CreatedDate = DateTime.UtcNow.AddHours(-retentionPeriod * 2);
            _helper.AddFileToStorage(file);

            _helper.SetStorageServiceResponse(DataFunction.DeleteAsync, false);

            // Act
            var response = await _helper.Service.CleanupAsync();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.OK);
            response.Data.Should().BeTrue();

            var files = _helper.GetStorageFiles();
            files.Should().HaveCount(1);

            // Teardown
            _helper.SetStorageServiceResponse(DataFunction.DeleteAsync, true);
        }
    }
}
