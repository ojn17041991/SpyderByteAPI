using FluentAssertions.Execution;
using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.DataAccess.UsersAccessorTests.Helpers;
using SpyderByteResources.Responses.Abstract;
using SpyderByteDataAccess.Models.Users;

namespace SpyderByteTest.DataAccess.UsersAccessorTests
{
    public class PatchAsyncTests
    {
        private readonly UsersAccessorHelper _helper;
        private readonly UsersAccessorExceptionHelper _exceptionHelper;

        public PatchAsyncTests()
        {
            _helper = new UsersAccessorHelper();
            _exceptionHelper = new UsersAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Patch_User_With_Game_Assignment_In_Accessor()
        {
            // Arrange
            var storedUser = await _helper.AddUser();
            var storedGame = await _helper.AddGame();
            var patchUser = _helper.GeneratePatchUser();
            patchUser.Id = storedUser.Id;
            patchUser.GameId = storedGame.Id;

            // Act
            var returnedUser = await _helper.Accessor.PatchAsync(patchUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.OK);
                returnedUser.Data.Should().NotBeNull();
                returnedUser.Data!.Id.Should().Be(patchUser.Id);
                returnedUser.Data!.UserGame!.GameId.Should().Be(patchUser.GameId!.Value);

                // Check the database.
                var updatedStoredUser = await _helper.GetUser(returnedUser.Data!.Id);
                updatedStoredUser.Should().NotBeNull();
                updatedStoredUser.Id.Should().Be(patchUser.Id);
                updatedStoredUser.UserGame!.GameId.Should().Be(patchUser.GameId!.Value);

                updatedStoredUser.Should().BeEquivalentTo(returnedUser.Data);
            }
        }

        [Fact]
        public async Task Can_Patch_User_Without_Existing_Game_Assignment_In_Accessor()
        {
            // Arrange
            var storedUser = await _helper.AddUserWithoutGame();
            var storedGame = await _helper.AddGame();
            var patchUser = _helper.GeneratePatchUser();
            patchUser.Id = storedUser.Id;
            patchUser.GameId = storedGame.Id;

            // Act
            var returnedUser = await _helper.Accessor.PatchAsync(patchUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.OK);
                returnedUser.Data.Should().NotBeNull();
                returnedUser.Data!.Id.Should().Be(patchUser.Id);
                returnedUser.Data!.UserGame!.GameId.Should().Be(patchUser.GameId!.Value);

                // Check the database.
                var updatedStoredUser = await _helper.GetUser(returnedUser.Data!.Id);
                updatedStoredUser.Should().NotBeNull();
                updatedStoredUser.Id.Should().Be(patchUser.Id);
                updatedStoredUser.UserGame!.GameId.Should().Be(patchUser.GameId!.Value);

                updatedStoredUser.Should().BeEquivalentTo(returnedUser.Data);
            }
        }

        [Fact]
        public async Task Can_Not_Patch_User_In_Accessor_If_User_Does_Not_Exist()
        {
            // Arrange
            var patchUser = _helper.GeneratePatchUser();

            // Act
            var returnedUser = await _helper.Accessor.PatchAsync(patchUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.NotFound);
                returnedUser.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Can_Not_Patch_User_In_Accessor_If_Assigned_Game_Does_Not_Exist()
        {
            // Arrange
            var storedUser = await _helper.AddUser();
            var patchUser = _helper.GeneratePatchUser();
            patchUser.Id = storedUser.Id;

            // Act
            var returnedUser = await _helper.Accessor.PatchAsync(patchUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.NotFound);
                returnedUser.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Can_Not_Patch_User_In_Accessor_If_Game_Is_Already_Assigned()
        {
            // Arrange
            var storedUser1 = await _helper.AddUser();
            var storedUser2 = await _helper.AddUser();
            var patchUser = _helper.GeneratePatchUser();
            patchUser.Id = storedUser1.Id;
            patchUser.GameId = storedUser2.UserGame!.GameId;

            // Act
            var returnedUser = await _helper.Accessor.PatchAsync(patchUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.AlreadyExists);
                returnedUser.Data.Should().BeNull();
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange
            var patchUser = _helper.GeneratePatchUser();

            // Act
            Func<Task<IDataResponse<User?>>> func = () => _exceptionHelper.Accessor.PatchAsync(patchUser);

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
