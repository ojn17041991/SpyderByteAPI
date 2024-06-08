using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.UsersServiceTests.Helpers;

namespace SpyderByteTest.Services.UsersServiceTests
{
    public class PostAsyncTests
    {
        private readonly UsersServiceHelper _helper;

        public PostAsyncTests()
        {
            _helper = new UsersServiceHelper();
        }

        [Fact]
        public async Task Can_Post_User_In_Service()
        {
            // Arrange
            var postUser = _helper.GeneratePostUser();
            postUser.UserType = UserType.Restricted;

            // Act
            var returnedUser = await _helper.Service.PostAsync(postUser);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.Created);
            returnedUser.Data.Should().NotBeNull();
            returnedUser.Data!.Should().BeEquivalentTo(postUser, options =>
                options
                    .Excluding(u => u.Password)
                    .Excluding(u => u.GameId)
            );
        }

        [Fact]
        public async Task Can_Not_Post_User_In_Service_If_User_Name_Already_Exists()
        {
            // Arrange
            var storedUser = _helper.AddUser(UserType.Restricted);
            var postUser = _helper.GeneratePostUser();
            postUser.UserName = storedUser.UserName;

            // Act
            var returnedUser = await _helper.Service.PostAsync(postUser);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.AlreadyExists);
            returnedUser.Data.Should().BeNull();
        }

        [Fact]
        public async Task Can_Not_Post_User_In_Service_If_User_Type_Is_Not_Restricted()
        {
            // Arrange
            var postUser = _helper.GeneratePostUser();
            postUser.UserType = UserType.Admin;

            // Act
            var returnedUser = await _helper.Service.PostAsync(postUser);

            // Assert
            returnedUser.Should().NotBeNull();
            returnedUser.Result.Should().Be(ModelResult.RequestInvalid);
            returnedUser.Data.Should().BeNull();
        }
    }
}
