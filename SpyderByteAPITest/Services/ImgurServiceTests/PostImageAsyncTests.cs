using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteAPI.Enums;
using SpyderByteAPITest.Services.ImgurServiceTests.Helper;

namespace SpyderByteAPITest.Services.ImgurServiceTests
{
    public class PostImageAsyncTests
    {
        private readonly ImgurServiceHelper _helper;

        public PostImageAsyncTests()
        {
            _helper = new ImgurServiceHelper();
        }

        [Fact]
        public async Task Can_Post_Image()
        {
            // Arrange
            _helper.BuildFullConfiguration();
            var formFile = _helper.GenerateFormFile();

            // Act
            var response = await _helper.Service.PostImageAsync(formFile, string.Empty, string.Empty);

            // Assert
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Result.Should().Be(ModelResult.OK);
                response.Data.Should().NotBeNull();
                response.Data.Url.Should().NotBeNullOrEmpty();
                response.Data.ImageId.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public async Task Can_Not_Post_Image_Without_Refresh_Token()
        {
            // Arrange
            _helper.BuildUnathorizedConfiguration();
            var formFile = _helper.GenerateFormFile();

            // Act
            var response = await _helper.Service.PostImageAsync(formFile, string.Empty, string.Empty);

            // Assert
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Result.Should().Be(ModelResult.Unauthorized);
                response.Data.Should().NotBeNull();
                response.Data.Url.Should().BeNullOrEmpty();
                response.Data.ImageId.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async Task Can_Not_Post_Image_When_Http_Request_Fails()
        {
            // Arrange
            _helper.BuildHttpRequestErrorConfiguration();
            var formFile = _helper.GenerateFormFile();

            // Act
            var response = await _helper.Service.PostImageAsync(formFile, string.Empty, string.Empty);

            // Assert
            using (new AssertionScope())
            {
                response.Should().NotBeNull();
                response.Result.Should().Be(ModelResult.Error);
                response.Data.Should().NotBeNull();
                response.Data.Url.Should().BeNullOrEmpty();
                response.Data.ImageId.Should().BeNullOrEmpty();
            }
        }
    }
}
