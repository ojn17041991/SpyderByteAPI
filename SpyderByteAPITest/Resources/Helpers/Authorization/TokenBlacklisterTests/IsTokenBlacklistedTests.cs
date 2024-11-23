using FluentAssertions;
using SpyderByteResources.Helpers.Authorization;

namespace SpyderByteTest.Resources.Helpers.Authorization.TokenBlacklisterTests
{
    [Collection("BlacklistSequential")]
    public class IsTokenBlacklistedTests
    {
        [Fact]
        public void Returns_True_If_Token_Is_In_Blacklist()
        {
            // Arrange
            TokenBlacklister.ClearBlacklist();
            string token = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token);

            // Act
            bool isTokenBlacklisted = TokenBlacklister.IsTokenBlacklisted(token);

            // Assert
            isTokenBlacklisted.Should().BeTrue();
        }

        [Fact]
        public void Returns_False_If_Token_Is_In_Blacklist()
        {
            // Arrange
            TokenBlacklister.ClearBlacklist();
            string token = Guid.NewGuid().ToString();

            // Act
            bool isTokenBlacklisted = TokenBlacklister.IsTokenBlacklisted(token);

            // Assert
            isTokenBlacklisted.Should().BeFalse();
        }
    }
}
