using FluentAssertions;
using SpyderByteResources.Helpers.Authorization;

namespace SpyderByteTest.Resources.Helpers.Authorization.TokenBlacklisterTests
{
    [Collection("BlacklistSequential")]
    public class GetBlacklistedTokensTests
    {
        [Fact]
        public void Can_Get_Tokens_From_Blacklist()
        {
            // Arrange
            TokenBlacklister.ClearBlacklist();
            string token1 = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token1);
            string token2 = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token2);
            string token3 = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(token3);

            // Act
            IList<string> tokens = TokenBlacklister.GetBlacklistedTokens();

            // Assert
            tokens.Should().HaveCount(3);
            tokens[0].Should().Be(token1);
            tokens[1].Should().Be(token2);
            tokens[2].Should().Be(token3);
        }
    }
}
