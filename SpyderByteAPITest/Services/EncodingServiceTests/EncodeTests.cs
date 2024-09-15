using FluentAssertions;
using SpyderByteTest.Services.EncodingServiceTests.Helpers;
using System.Security.Claims;

namespace SpyderByteTest.Services.EncodingServiceTests
{
    public class EncodeTests
    {
        private readonly EncodingServiceHelper helper;

        public EncodeTests()
        {
            helper = new EncodingServiceHelper();
        }

        [Fact]
        public void Can_Encode_Claims_To_Token()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("Name", "Value")
            };

            // Act
            string token = helper.Service.Encode(claims);

            // Assert
            token.Should().NotBeEmpty();
        }
    }
}
