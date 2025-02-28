using FluentAssertions;
using SpyderByteResources.Helpers.Authorization;

namespace SpyderByteTest.Resources.Helpers.Authorization.TokenBlacklisterTests
{
    [Collection("BlacklistSequential")]
    public class ClearBlacklistTests
    {
        [Fact]
        public void Can_Clear_Token_Blacklist()
        {
            // Arrange
            string token1 = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token1);
            string token2 = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token2);
            string token3 = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token3);

            // Act
            TokenBlacklister.ClearBlacklist();

            // Assert
            IList<string> tokens = TokenBlacklister.GetBlacklistedTokens();
            tokens.Should().BeEmpty();
        }
    }
}
