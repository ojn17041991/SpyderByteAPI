using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.StorageServiceTests.Helpers;

namespace SpyderByteTest.Services.StorageServiceTests
{
    public class GetFilesAsyncTests
    {
        private readonly StorageServiceHelper helper;

        public GetFilesAsyncTests()
        {
            helper = new StorageServiceHelper();
        }

        [Fact]
        public async Task Can_Get_Files()
        {
            // Arrange
            var blob1 = helper.AddBlob("blob1");
            var blob2 = helper.AddBlob("blob2");
            var blob3 = helper.AddBlob("blob3");

            helper.SetContainerExists(true);
            helper.SetIsResponseError(false);

            // Act
            var getFilesResponse = await helper.Service.GetFilesAsync();

            // Assert
            getFilesResponse.Should().NotBeNull();
            getFilesResponse.Result.Should().Be(ModelResult.OK);
            getFilesResponse.Data.Should().HaveCount(3);
            getFilesResponse.Data![0].FileName.Should().Be(blob1.Name);
            getFilesResponse.Data![0].CreatedDate.Ticks.Should().Be(blob1.Properties.CreatedOn!.Value.Ticks);
            getFilesResponse.Data![1].FileName.Should().Be(blob2.Name);
            getFilesResponse.Data![1].CreatedDate.Ticks.Should().Be(blob2.Properties.CreatedOn!.Value.Ticks);
            getFilesResponse.Data![2].FileName.Should().Be(blob3.Name);
            getFilesResponse.Data![2].CreatedDate.Ticks.Should().Be(blob3.Properties.CreatedOn!.Value.Ticks);
        }

        [Fact]
        public async Task Can_Not_Get_Files_When_Container_Does_Not_Exist()
        {
            // Arrange
            helper.SetContainerExists(false);
            helper.SetIsResponseError(false);

            // Act
            var getFilesResponse = await helper.Service.GetFilesAsync();

            // Assert
            getFilesResponse.Should().NotBeNull();
            getFilesResponse.Result.Should().Be(ModelResult.NotFound);
            getFilesResponse.Data.Should().BeNull();
        }
    }
}
