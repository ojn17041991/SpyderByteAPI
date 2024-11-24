using FluentAssertions;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Resources;

namespace SpyderByteTest.API.Text
{
    public class ApiResourcesTests
    {
        [Fact]
        public void Can_Get_Api_Resources()
        {
            // Arrange
            IStringLookup<string> apiResources = new ApiResources();
            string titleKey = "title";
            string descriptionKey = "description";

            // Act
            string title = apiResources.GetResource(titleKey);
            string description = apiResources.GetResource(descriptionKey);

            // Assert
            title.Should().NotBeEmpty();
            description.Should().NotBeEmpty();
        }

        [Fact]
        public void Empty_String_Returned_When_Resource_Not_Found()
        {
            // Arrange
            IStringLookup<string> apiResources = new ApiResources();
            string versionKey = "version";

            // Act
            string version = apiResources.GetResource(versionKey);

            // Assert
            version.Should().BeEmpty();
        }
    }
}
