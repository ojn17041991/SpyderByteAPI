using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteServices.Models.Authentication;
using SpyderByteTest.Services.AuthenticationServiceTests.Helpers;

namespace SpyderByteTest.Services.AuthenticationServiceTests
{
    public class AuthenticateAsyncTests
    {
        private readonly AuthenticationServiceHelper helper;

        public AuthenticateAsyncTests()
        {
            helper = new AuthenticationServiceHelper();
        }

        [Fact]
        public async Task Can_Not_Authenticate_When_User_Does_Not_Exist()
        {
            // Arrange
            var login = new Login
            {
                UserName = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

            // Act
            var authenticationResponse = await helper.Service.AuthenticateAsync(login);

            // Assert
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.Result.Should().Be(ModelResult.Unauthorized);
            authenticationResponse.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Can_Not_Authenticate_When_Password_Is_Incorrect()
        {
            // Arrange
            var login = helper.AddLogin(UserType.Admin);
            var invalidLogin = new Login
            {
                UserName = login.UserName,
                Password = Guid.NewGuid().ToString()
            };

            // Act
            var authenticationResponse = await helper.Service.AuthenticateAsync(invalidLogin);

            // Assert
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.Result.Should().Be(ModelResult.Unauthorized);
            authenticationResponse.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Can_Not_Authenticate_When_Claims_Cannot_Be_Encoded()
        {
            // Arrange
            var login = helper.AddLogin(UserType.Admin);
            helper.SetUseEmptyEncodingToken(true);

            // Act
            var authenticationResponse = await helper.Service.AuthenticateAsync(login);

            // Assert
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.Result.Should().Be(ModelResult.Unauthorized);
            authenticationResponse.Data.Should().BeEmpty();

            // Cleanup
            helper.SetUseEmptyEncodingToken(false);
        }

        [Fact]
        public async Task Can_Authenticate_Utility()
        {
            // Arrange
            var login = helper.AddLogin(UserType.Utility);

            // Act
            var authenticationResponse = await helper.Service.AuthenticateAsync(login);

            // Assert
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.Result.Should().Be(ModelResult.OK);
            authenticationResponse.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Can_Authenticate_Restricted()
        {
            // Arrange
            var login = helper.AddLogin(UserType.Restricted);

            // Act
            var authenticationResponse = await helper.Service.AuthenticateAsync(login);

            // Assert
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.Result.Should().Be(ModelResult.OK);
            authenticationResponse.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Can_Authenticate_Administrator()
        {
            // Arrange
            var login = helper.AddLogin(UserType.Admin);

            // Act
            var authenticationResponse = await helper.Service.AuthenticateAsync(login);

            // Assert
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.Result.Should().Be(ModelResult.OK);
            authenticationResponse.Data.Should().NotBeEmpty();
        }
    }
}
