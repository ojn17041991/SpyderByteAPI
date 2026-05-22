using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SpyderByteResources.Extensions;

namespace SpyderByteTest.Resources.Helpers.Extensions.IFormFileExtensionsTests
{
    public class GetBytesTests
    {
        [Fact]
        public async Task Can_Get_Bytes_From_Form_File()
        {
            // Arrange
            string content = "Hello World";
            byte[] expected = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(expected);
            IFormFile file = new FormFile(stream, 0, expected.Length, "test", "test.txt");

            // Act
            byte[] actual = await file.GetBytes();

            // Assert
            actual.Should().Equal(expected);
        }

        [Fact]
        public async Task Can_Get_Bytes_When_Form_File_Is_Empty()
        {
            // Arrange
            var stream = new MemoryStream(Array.Empty<byte>());
            IFormFile file = new FormFile(stream, 0, 0, "test", "test.txt");

            // Act
            byte[] actual = await file.GetBytes();

            // Assert
            actual.Should().BeEmpty();
        }
    }
}
