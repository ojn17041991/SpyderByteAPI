using Azure.Storage.Blobs.Models;
using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteServices.Models.Data;
using SpyderByteTest.Services.StorageServiceTests.Enums;
using SpyderByteTest.Services.StorageServiceTests.Helpers;

namespace SpyderByteTest.Services.StorageServiceTests
{
    public class DeleteAsyncTests
    {
        private readonly StorageServiceHelper helper;

        public DeleteAsyncTests()
        {
            helper = new StorageServiceHelper();
        }

        [Fact]
        public async Task Can_Delete_File()
        {
            // Arrange
            BlobItem blob1 = helper.AddBlob("blob1");
            StorageFile file1 = helper.ConvertBlobToStorageFile(blob1);
            BlobItem blob2 = helper.AddBlob("blob2");
            StorageFile file2 = helper.ConvertBlobToStorageFile(blob2);

            helper.SetContainerExists(true);
            helper.SetBlobExists(true);
            helper.SetIsResponseError(StorageFunction.DeleteAsync, false);
            helper.PrepareNextBlobForDeletion(blob1);

            // Act
            var deleteResponse = await helper.Service.DeleteAsync(file1);

            // Assert
            deleteResponse.Should().NotBeNull();
            deleteResponse.Result.Should().Be(ModelResult.OK);
            deleteResponse.Data.Should().BeTrue();

            var blobs = helper.GetBlobs();
            blobs.Should().HaveCount(1);
            blobs[0].Name.Should().Be(blob2.Name);
        }

        [Fact]
        public async Task Can_Not_Delete_File_When_Container_Does_Not_Exist()
        {
            // Arrange
            BlobItem blob = helper.AddBlob("blob");
            StorageFile file = helper.ConvertBlobToStorageFile(blob);
            helper.SetContainerExists(false);
            helper.SetBlobExists(true);
            helper.SetIsResponseError(StorageFunction.DeleteAsync, false);

            // Act
            var deleteResponse = await helper.Service.DeleteAsync(file);

            // Assert
            deleteResponse.Should().NotBeNull();
            deleteResponse.Result.Should().Be(ModelResult.NotFound);
            deleteResponse.Data.Should().BeFalse();
        }

        [Fact]
        public async Task Can_Not_Delete_File_When_Blob_Does_Not_Exist()
        {
            // Arrange
            BlobItem blob = helper.AddBlob("blob");
            StorageFile file = helper.ConvertBlobToStorageFile(blob);
            helper.SetContainerExists(true);
            helper.SetBlobExists(false);
            helper.SetIsResponseError(StorageFunction.DeleteAsync, false);

            // Act
            var deleteResponse = await helper.Service.DeleteAsync(file);

            // Assert
            deleteResponse.Should().NotBeNull();
            deleteResponse.Result.Should().Be(ModelResult.NotFound);
            deleteResponse.Data.Should().BeFalse();
        }

        [Fact]
        public async Task Can_Not_Delete_File_When_Response_Indicates_Error()
        {
            // Arrange
            BlobItem blob = helper.AddBlob("blob");
            StorageFile file = helper.ConvertBlobToStorageFile(blob);
            helper.SetContainerExists(true);
            helper.SetBlobExists(true);
            helper.SetIsResponseError(StorageFunction.DeleteAsync, true);

            // Act
            var deleteResponse = await helper.Service.DeleteAsync(file);

            // Assert
            deleteResponse.Should().NotBeNull();
            deleteResponse.Result.Should().Be(ModelResult.Error);
            deleteResponse.Data.Should().BeFalse();
        }
    }
}
