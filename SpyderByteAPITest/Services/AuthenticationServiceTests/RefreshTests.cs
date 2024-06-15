using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.AuthenticationServiceTests.Helpers;

namespace SpyderByteTest.Services.AuthenticationServiceTests
{
    public class RefreshTests
    {
        private readonly AuthenticationServiceHelper helper;

        public RefreshTests()
        {
            helper = new AuthenticationServiceHelper();
        }

        [Fact]
        public void Can_Not_Refresh_Token_When_Current_Token_Is_Empty()
        {
            // Arrange
            helper.SetUseEmptyBearerToken(true);
            var httpContext = helper.GenerateHttpContext();

            // Act
            var refreshResponse = helper.Service.Refresh(httpContext);

            // Assert
            refreshResponse.Should().NotBeNull();
            refreshResponse.Result.Should().Be(ModelResult.Error);
            refreshResponse.Data.Should().BeEmpty();

            // Cleanup
            helper.SetUseEmptyBearerToken(false);
        }

        [Fact]
        public void Can_Not_Refresh_Token_When_Current_Token_Cannot_Be_Decoded()
        {
            // Arrange
            helper.SetUseEmptyDecodingToken(true);
            var httpContext = helper.GenerateHttpContext();

            // Act
            var refreshResponse = helper.Service.Refresh(httpContext);

            // Assert
            refreshResponse.Should().NotBeNull();
            refreshResponse.Result.Should().Be(ModelResult.Error);
            refreshResponse.Data.Should().BeEmpty();

            // Cleanup
            helper.SetUseEmptyDecodingToken(false);
        }

        [Fact]
        public void Can_Not_Refresh_Token_When_New_Token_Cannot_Be_Encoded()
        {
            // Arrange
            helper.SetUseEmptyEncodingToken(true);
            var httpContext = helper.GenerateHttpContext();

            // Act
            var refreshResponse = helper.Service.Refresh(httpContext);

            // Assert
            refreshResponse.Should().NotBeNull();
            refreshResponse.Result.Should().Be(ModelResult.Error);
            refreshResponse.Data.Should().BeEmpty();

            // Cleanup
            helper.SetUseEmptyEncodingToken(false);
        }

        [Fact]
        public void Can_Refresh_Token()
        {
            // Arrange
            var httpContext = helper.GenerateHttpContext();

            // Act
            var refreshResponse = helper.Service.Refresh(httpContext);

            // Assert
            refreshResponse.Should().NotBeNull();
            refreshResponse.Result.Should().Be(ModelResult.OK);
            refreshResponse.Data.Should().NotBeEmpty();
        }
    }
}
