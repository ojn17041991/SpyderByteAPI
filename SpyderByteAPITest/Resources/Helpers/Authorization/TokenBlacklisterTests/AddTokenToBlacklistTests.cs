using FluentAssertions;
using SpyderByteResources.Helpers.Authorization;

namespace SpyderByteTest.Resources.Helpers.Authorization.TokenBlacklisterTests
{
    [Collection("BlacklistSequential")]
    public class AddTokenToBlacklistTests
    {
        [Fact]
        public void Can_Add_Token_To_Blacklist()
        {
            // Arrange
            TokenBlacklister.ClearBlacklist();
            string token = Guid.NewGuid().ToString();

            // Act
            TokenBlacklister.AddTokenToBlacklist(token);

            // Assert
            IList<string> tokens = TokenBlacklister.GetBlacklistedTokens();
            tokens.Should().HaveCount(1);
            tokens.Single().Should().Be(token);
        }

        [Fact]
        public void Token_Overwrites_Oldest_Token_If_Limit_Exceeded()
        {
            // Arrange
            TokenBlacklister.ClearBlacklist();
            int iterations = TokenBlacklister.MaxTokensStored;

            // Act
            string firstToken = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(firstToken);

            for (int i = 1; i < iterations; i++)
            {
                string token = Guid.NewGuid().ToString();
                TokenBlacklister.AddTokenToBlacklist(token);
            }

            string lastToken = Guid.NewGuid().ToString();
            TokenBlacklister.AddTokenToBlacklist(lastToken);

            // Assert
            IList<string> tokens = TokenBlacklister.GetBlacklistedTokens();
            tokens.Should().HaveCount(iterations);
            tokens.Should().NotContain(firstToken);
            tokens.Last().Should().Be(lastToken);
        }
    }
}
