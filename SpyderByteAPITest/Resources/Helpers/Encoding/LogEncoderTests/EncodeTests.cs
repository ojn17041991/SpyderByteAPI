using FluentAssertions;
using SpyderByteResources.Helpers.Encoding;

namespace SpyderByteTest.Resources.Helpers.Encoding.LogEncoderTests
{
    public class EncodeTests
    {
        [Fact]
        public void Encoded_String_Removes_Dangerous_Characters()
        {
            // Arrange
            string input = @"
<html>
    <p>
        Dummy HTML.
    </p>
<\html>";
            string expected = "html    p        Dummy HTML.    phtml";

            // Act
            string actual = LogEncoder.Encode(input);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
