using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using SpyderByteTest.DataAccess.UsersAccessorTests.Helpers;

namespace SpyderByteTest.DataAccess.UsersAccessorTests
{
    public class DeleteAsyncTests
    {
        private readonly UsersAccessorHelper _helper;
        private readonly UsersAccessorExceptionHelper _exceptionHelper;

        public DeleteAsyncTests()
        {
            _helper = new UsersAccessorHelper();
            _exceptionHelper = new UsersAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Delete_User_In_Accessor()
        {
            // Arrange
            var storedUser = await _helper.AddUser();
            var preTestUsers = await _helper.GetUsers();

            // Act
            var returnedUser = await _helper.Accessor.DeleteAsync(storedUser.Id);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.OK);
                returnedUser.Data.Should().BeEquivalentTo(storedUser);

                // Check the database.
                var postTestUsers = await _helper.GetUsers();
                postTestUsers.Should().HaveCount(preTestUsers.Count - 1);
            }
        }

        [Fact]
        public async Task Can_Not_Delete_User_In_Accessor_If_User_Does_Not_Exist()
        {
            // Arrange
            var preTestUsers = await _helper.GetUsers();

            // Act
            var returnedUser = await _helper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.NotFound);
                returnedUser.Data.Should().BeNull();

                // Check the database.
                var postTestUsers = await _helper.GetUsers();
                postTestUsers.Should().HaveCount(preTestUsers.Count);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<User?>>> func = () => _exceptionHelper.Accessor.DeleteAsync(Guid.NewGuid());

            // Assert
            using (new AssertionScope())
            {
                var leaderboardRecords = await func.Invoke();
                leaderboardRecords?.Should().NotBeNull();
                leaderboardRecords?.Result.Should().Be(ModelResult.Error);
                leaderboardRecords?.Data?.Should().BeNull();
            }
        }
    }
}
