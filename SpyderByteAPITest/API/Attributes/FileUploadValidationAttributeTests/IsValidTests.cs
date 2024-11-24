using FluentAssertions;
using SpyderByteAPI.Attributes;
using SpyderByteTest.API.Attributes.FileUploadValidationAttributeTests.Helpers;

namespace SpyderByteTest.API.Attributes.FileUploadValidationAttributeTests
{
    public class IsValidTests
    {
        private readonly AttributeHelper helper;

        public IsValidTests()
        {
            helper = new AttributeHelper();
        }

        [Fact]
        public void Returns_True_If_File_Type_Is_Png()
        {
            // Arrange
            var attribute = new FileUploadValidationAttribute();
            var formFile = helper.GeneratePngFile();

            // Act
            bool isValid = attribute.IsValid(formFile);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Returns_False_If_File_Type_Is_Png_But_Header_Bytes_Are_Incorrect()
        {
            // Arrange
            var attribute = new FileUploadValidationAttribute();
            var formFile = helper.GeneratePngFileWithIncorrectHeaderBytes();

            // Act
            bool isValid = attribute.IsValid(formFile);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void Returns_False_If_File_Type_Is_Not_Png()
        {
            // Arrange
            var attribute = new FileUploadValidationAttribute();
            var formFile = helper.GeneratePdfFile();

            // Act
            bool isValid = attribute.IsValid(formFile);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}
