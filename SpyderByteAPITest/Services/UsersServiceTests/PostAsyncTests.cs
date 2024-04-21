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
    }
}
