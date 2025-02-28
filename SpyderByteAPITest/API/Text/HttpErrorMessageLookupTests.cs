using FluentAssertions;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Resources;

namespace SpyderByteTest.API.Text
{
    public class HttpErrorMessageLookupTests
    {
        [Fact]
        public void Can_Get_Not_Found_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.NotFound;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Already_Exists_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.AlreadyExists;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Request_Data_Incomplete_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.RequestDataIncomplete;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Request_Invalid_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.RequestInvalid;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Relationship_Violation_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.RelationshipViolation;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Empty_String_Returned_When_Resource_Not_Found()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.OK;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().BeEmpty();
        }
    }
}
