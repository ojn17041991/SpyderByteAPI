using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.AuthenticationServiceTests.Helpers;

namespace SpyderByteTest.Services.AuthenticationServiceTests
{
    public class DeauthenticateTests
    {
        private readonly AuthenticationServiceHelper helper;

        public DeauthenticateTests()
        {
            helper = new AuthenticationServiceHelper();
        }

        [Fact]
        public void Can_Not_Deauthenticate_When_Token_Is_Empty()
        {
            // Arrange
            helper.SetUseEmptyBearerToken(true);
            var httpContext = helper.GenerateHttpContext();

            // Act
            var deauthenticationResponse = helper.Service.Deauthenticate(httpContext);

            // Assert
            deauthenticationResponse.Should().NotBeNull();
            deauthenticationResponse.Result.Should().Be(ModelResult.Error);
            deauthenticationResponse.Data.Should().BeEmpty();

            // Cleanup
            helper.SetUseEmptyBearerToken(false);
        }

        [Fact]
        public void Can_Deauthenticate()
        {
            // Arrange
            var httpContext = helper.GenerateHttpContext();

            // Act
            var deauthenticationResponse = helper.Service.Deauthenticate(httpContext);

            // Assert
            deauthenticationResponse.Should().NotBeNull();
            deauthenticationResponse.Result.Should().Be(ModelResult.OK);
            deauthenticationResponse.Data.Should().NotBeEmpty();
        }
    }
}
