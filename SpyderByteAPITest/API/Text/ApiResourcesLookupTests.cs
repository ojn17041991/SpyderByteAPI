using FluentAssertions;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Resources;

namespace SpyderByteTest.API.Text
{
    public class ApiResourcesLookupTests
    {
        [Fact]
        public void Can_Get_Api_Title()
        {
            // Arrange
            IStringLookup<string> apiResources = new ApiResourceLookup();
            string titleKey = "title";

            // Act
            string title = apiResources.GetResource(titleKey);

            // Assert
            title.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Get_Api_Description()
        {
            // Arrange
            IStringLookup<string> apiResources = new ApiResourceLookup();
            string descriptionKey = "description";

            // Act
            string description = apiResources.GetResource(descriptionKey);

            // Assert
            description.Should().NotBeEmpty();
        }

        [Fact]
        public void Empty_String_Returned_When_Resource_Not_Found()
        {
            // Arrange
            IStringLookup<string> apiResources = new ApiResourceLookup();
            string versionKey = "version";

            // Act
            string version = apiResources.GetResource(versionKey);

            // Assert
            version.Should().BeEmpty();
        }
    }
}
