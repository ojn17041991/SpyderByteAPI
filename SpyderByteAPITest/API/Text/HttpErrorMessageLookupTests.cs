using FluentAssertions;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Resources;

namespace SpyderByteTest.API.Text
{
    public class HttpErrorMessageLookupTests
    {
        [Fact]
        public void Can_Get_Ok_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.OK;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Created_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.Created;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

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
        public void Can_Get_Image_Deletion_Failed_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.ImageDeletionFailed;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Forbidden_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.Forbidden;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Unauthorized_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.Unauthorized;

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Error_Message()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            ModelResult modelResult = ModelResult.Error;

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
            ModelResult modelResult = (ModelResult)(-1);

            // Act
            string message = httpErrorMessages.GetResource(modelResult);

            // Assert
            message.Should().BeEmpty();
        }

        [Fact]
        public void All_Model_Results_Are_Supported()
        {
            // Arrange
            IStringLookup<ModelResult> httpErrorMessages = new HttpErrorMessageLookup();
            IDictionary<ModelResult, string> results = new Dictionary<ModelResult, string>();
            ModelResult[] modelResults = Enum.GetValues<ModelResult>();

            // Act
            foreach (ModelResult modelResult in modelResults)
            {
                results.Add(modelResult, httpErrorMessages.GetResource(modelResult));
            }

            // Assert
            foreach (var result in results)
            {
                result.Value.Should().NotBeNullOrWhiteSpace($"because {result.Key} does not have an associated message.");
            }
        }
    }
}
