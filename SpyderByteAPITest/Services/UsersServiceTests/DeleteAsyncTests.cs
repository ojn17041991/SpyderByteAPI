using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.UsersServiceTests.Helpers;

namespace SpyderByteTest.Services.UsersServiceTests
{
    public class DeleteAsyncTests
    {
        private readonly UsersServiceHelper _helper;

        public DeleteAsyncTests()
        {
            _helper = new UsersServiceHelper();
        }

        [Fact]
        public async Task Can_Delete_User_From_Service()
        {
            // Arrange
            var storedUser = _helper.AddUser(UserType.Restricted);

            // Act
            var returnedUser = await _helper.Service.DeleteAsync(storedUser.Id);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.OK);
            returnedUser.Data.Should().NotBeNull();
            returnedUser.Data!.Should().BeEquivalentTo(storedUser);
        }

        [Fact]
        public async Task Can_Delete_User_From_Service_If_User_Does_Not_Exist()
        {
            // Arrange

            // Act
            var returnedUser = await _helper.Service.DeleteAsync(Guid.NewGuid());

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.NotFound);
            returnedUser.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Delete_User_From_Service_If_User_Type_Is_Not_Restricted()
        {
            // Arrange
            var storedUser = _helper.AddUser(UserType.Admin);

            // Act
            var returnedUser = await _helper.Service.DeleteAsync(storedUser.Id);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.RequestInvalid);
            returnedUser.Data.Should().BeNull();
        }
    }
}
