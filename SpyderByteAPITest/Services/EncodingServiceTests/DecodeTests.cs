using FluentAssertions;
using SpyderByteTest.Services.EncodingServiceTests.Helpers;

namespace SpyderByteTest.Services.EncodingServiceTests
{
    public class DecodeTests
    {
        private readonly EncodingServiceHelper helper;

        public DecodeTests()
        {
            helper = new EncodingServiceHelper();
        }

        [Fact]
        public void Can_Decode_Token_To_Claims()
        {
            // Arrange
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJOYW1lIjoiVmFsdWUiLCJuYmYiOjE3MTg0NTIyNDUsImV4cCI6MTcxODQ1MjMwNSwiaWF0IjoxNzE4NDUyMjQ1LCJpc3MiOiJUZXN0IiwiYXVkIjoiVGVzdCJ9.y1m-TFYqLZb6EfWYvY79nnfmq1oKQGhPK2zaJd1uDOE";

            // Act
            var claims = helper.Service.Decode(token);

            // Assert
            claims.Should().NotBeNullOrEmpty();
        }
    }
}
