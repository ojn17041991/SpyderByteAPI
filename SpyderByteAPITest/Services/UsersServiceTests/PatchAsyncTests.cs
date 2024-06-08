using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.UsersServiceTests.Helpers;

namespace SpyderByteTest.Services.UsersServiceTests
{
    public class PatchAsyncTests
    {
        private readonly UsersServiceHelper _helper;

        public PatchAsyncTests()
        {
            _helper = new UsersServiceHelper();
        }

        [Fact]
        public async Task Can_Patch_User_In_Service()
        {
            // Arrange
            var storedUser = _helper.AddUser(UserType.Restricted);
            var patchUser = _helper.GeneratePatchUser();
            patchUser.Id = storedUser.Id;

            // Act
            var returnedUser = await _helper.Service.PatchAsync(patchUser);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.OK);
            returnedUser.Data.Should().NotBeNull();
            returnedUser.Data!.Should().BeEquivalentTo(patchUser, options => options.Excluding(u => u.GameId));
            returnedUser.Data!.UserGame!.GameId.Should().Be(patchUser.GameId!.Value);
        }

        [Fact]
        public async Task Can_Not_Patch_User_In_Service_If_User_Does_Not_Exist()
        {
            // Arrange
            var patchUser = _helper.GeneratePatchUser();

            // Act
            var returnedUser = await _helper.Service.PatchAsync(patchUser);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.NotFound);
            returnedUser.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Not_Patch_User_In_Service_If_User_Type_Is_Not_Restricted()
        {
            // Arrange
            var storedUser = _helper.AddUser(UserType.Admin);
            var patchUser = _helper.GeneratePatchUser();
            patchUser.Id = storedUser.Id;

            // Act
            var returnedUser = await _helper.Service.PatchAsync(patchUser);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.RequestInvalid);
            returnedUser.Data.Should().BeNull();
        }
    }
}
