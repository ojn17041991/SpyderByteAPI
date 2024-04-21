using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.UsersServiceTests.Helpers;

namespace SpyderByteTest.Services.UsersServiceTests
{
    public class GetAsyncTests
    {
        private readonly UsersServiceHelper _helper;

        public GetAsyncTests()
        {
            _helper = new UsersServiceHelper();
        }

        [Fact]
        public async Task Can_Get_Users_From_Service()
        {
            // Arrange
            var storedUser = _helper.AddUser();

            // Act
            var returnedUser = await _helper.Service.GetAsync(storedUser.Id);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.OK);
            returnedUser.Data.Should().NotBeNull();
            returnedUser.Data!.Should().BeEquivalentTo(storedUser);
        }
    }
}
