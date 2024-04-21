using FluentAssertions.Execution;
using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.DataAccess.UsersAccessorTests.Helpers;
using SpyderByteResources.Responses.Abstract;
using SpyderByteDataAccess.Models.Users;

namespace SpyderByteTest.DataAccess.UsersAccessorTests
{
    public class GetByUserNameAsyncTests
    {
        private readonly UsersAccessorHelper _helper;
        private readonly UsersAccessorExceptionHelper _exceptionHelper;

        public GetByUserNameAsyncTests()
        {
            _helper = new UsersAccessorHelper();
            _exceptionHelper = new UsersAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_User_Record_By_User_Name_From_Accessor()
        {
            // Arrange
            var storedUser = await _helper.AddUser();

            // Act
            var returnedUser = await _helper.Accessor.GetByUserNameAsync(storedUser.UserName);

            // Assert
            using (new AssertionScope())
            {
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.OK);
                returnedUser.Data.Should().NotBeNull();
                returnedUser.Data.Should().BeEquivalentTo(storedUser);
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<User?>>> func = () => _exceptionHelper.Accessor.GetByUserNameAsync(string.Empty);

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
