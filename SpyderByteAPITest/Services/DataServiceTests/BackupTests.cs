using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.DataServiceTests.Helpers;

namespace SpyderByteTest.Services.DataServiceTests
{
    public class BackupTests
    {
        private readonly DataServiceHelper _helper;

        public BackupTests()
        {
            _helper = new DataServiceHelper();
        }

        [Fact]
        public async Task Can_Backup_Data()
        {
            // Arrange
            _helper.BuildFullConfiguration();
            _helper.BuildDatabaseDirectory();
            _helper.BuildDatabaseFile();

            // Act
            var response = await _helper.Service.Backup();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.OK);
            response.Data.Should().BeTrue();

            // Teardown
            _helper.ClearDatabaseFile();
            _helper.ClearDatabaseDirectory();
        }

        [Fact]
        public async Task Can_Not_Backup_Data_When_Configuration_Is_Invalid()
        {
            // Arrange

            // Act
            var response = await _helper.Service.Backup();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.Error);
            response.Data.Should().BeFalse();
        }

        [Fact]
        public async Task Can_Not_Backup_Data_When_Database_Directory_Is_Missing()
        {
            // Arrange
            _helper.BuildFullConfiguration();

            // Act
            var response = await _helper.Service.Backup();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.NotFound);
            response.Data.Should().BeFalse();
        }

        [Fact]
        public async Task Can_Not_Backup_Data_When_Database_File_Is_Missing()
        {
            // Arrange
            _helper.BuildFullConfiguration();
            _helper.BuildDatabaseDirectory();

            // Act
            var response = await _helper.Service.Backup();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.NotFound);
            response.Data.Should().BeFalse();

            // Teardown
            _helper.ClearDatabaseDirectory();
        }

        [Fact]
        public async Task Can_Not_Backup_Data_When_Storage_Request_Fails()
        {
            // Arrange
            _helper.BuildFullConfiguration();
            _helper.BuildDatabaseDirectory();
            _helper.BuildDatabaseFile();
            _helper.SetStorageServiceResponse(false);

            // Act
            var response = await _helper.Service.Backup();

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().Be(ModelResult.Error);
            response.Data.Should().BeFalse();

            // Teardown
            _helper.ClearDatabaseFile();
            _helper.ClearDatabaseDirectory();
            _helper.SetStorageServiceResponse(true);
        }
    }
}
