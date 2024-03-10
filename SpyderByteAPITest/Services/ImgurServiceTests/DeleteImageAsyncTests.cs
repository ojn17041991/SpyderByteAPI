using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPITest.Services.ImgurServiceTests.Helper;
using SpyderByteResources.Enums;

namespace SpyderByteAPITest.Services.ImgurServiceTests
{
    public class DeleteImageAsyncTests
    {
        private readonly ImgurServiceHelper _helper;

        public DeleteImageAsyncTests()
        {
            _helper = new ImgurServiceHelper();
        }

        [Fact]
        public async Task Can_Delete_Image()
        {
            // Arrange
            _helper.BuildFullConfiguration();

            // Act
            var response = await _helper.Service.DeleteImageAsync(Guid.NewGuid().ToString());

            // Assert
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Result.Should().Be(ModelResult.OK);
                response.Data.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Can_Not_Delete_Image_Without_Refresh_Token()
        {
            // Arrange
            _helper.BuildUnathorizedConfiguration();

            // Act
            var response = await _helper.Service.DeleteImageAsync(Guid.NewGuid().ToString());

            // Assert
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Result.Should().Be(ModelResult.Unauthorized);
                response.Data.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Can_Not_Post_Image_When_Http_Request_Fails()
        {
            // Arrange
            _helper.BuildHttpRequestErrorConfiguration();

            // Act
            var response = await _helper.Service.DeleteImageAsync(Guid.NewGuid().ToString());

            // Assert
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Result.Should().Be(ModelResult.Error);
                response.Data.Should().BeFalse();
            }
        }
    }
}
