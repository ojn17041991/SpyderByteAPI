using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SpyderByteResources.Extensions;
using SpyderByteTest.Extensions;

namespace SpyderByteTest.Resources.Helpers.Extensions.IFormFileExtensionsTests
{
    public class GetStreamTests
    {
        [Fact]
        public async Task Can_Get_Stream_From_Form_File()
        {
            // Arrange
            string content = "Hello World";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var expected = new MemoryStream(bytes);
            IFormFile file = new FormFile(expected, 0, bytes.Length, "test", "test.txt");

            // Act
            var actual = await file.GetStream();

            // Assert
            actual.GetBytes().Should().BeSameAs(expected.GetBytes());
        }

        [Fact]
        public async Task Can_Get_Stream_When_Form_File_Is_Empty()
        {
            // Arrange
            var stream = new MemoryStream(Array.Empty<byte>());
            IFormFile file = new FormFile(stream, 0, 0, "test", "test.txt");

            // Act
            var actual = await file.GetStream();

            // Assert
            actual.GetBytes().Should().BeEmpty();
        }
    }
}
