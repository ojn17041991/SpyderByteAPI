using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.StorageServiceTests.Helpers;
using System.Text;

namespace SpyderByteTest.Services.StorageServiceTests
{
    public class UploadAsyncTests
    {
        private readonly StorageServiceHelper helper;

        public UploadAsyncTests()
        {
            helper = new StorageServiceHelper();
        }

        [Fact]
        public async Task Can_Upload_File()
        {
            // Arrange
            string fileName = ".txt";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            helper.SetContainerExists(true);
            helper.SetIsResponseError(false);

            // Act
            var uploadResponse = await helper.Service.UploadAsync(fileName, stream);

            // Assert
            uploadResponse.Should().NotBeNull();
            uploadResponse.Result.Should().Be(ModelResult.OK);
            uploadResponse.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Can_Not_Upload_File_When_Container_Does_Not_Exist()
        {
            // Arrange
            string fileName = ".txt";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            helper.SetContainerExists(false);
            helper.SetIsResponseError(false);

            // Act
            var uploadResponse = await helper.Service.UploadAsync(fileName, stream);

            // Assert
            uploadResponse.Should().NotBeNull();
            uploadResponse.Result.Should().Be(ModelResult.NotFound);
            uploadResponse.Data.Should().BeFalse();
        }

        [Fact]
        public async Task Can_Not_Upload_File_When_Response_Indicates_Error()
        {
            // Arrange
            string fileName = ".txt";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            helper.SetContainerExists(true);
            helper.SetIsResponseError(true);

            // Act
            var uploadResponse = await helper.Service.UploadAsync(fileName, stream);

            // Assert
            uploadResponse.Should().NotBeNull();
            uploadResponse.Result.Should().Be(ModelResult.Error);
            uploadResponse.Data.Should().BeFalse();
        }
    }
}
