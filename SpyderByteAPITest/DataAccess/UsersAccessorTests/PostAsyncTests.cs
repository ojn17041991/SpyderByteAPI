using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteTest.DataAccess.UsersAccessorTests.Helpers;

namespace SpyderByteTest.DataAccess.UsersAccessorTests
{
    public class PostAsyncTests
    {
        private readonly UsersAccessorHelper _helper;
        private readonly UsersAccessorExceptionHelper _exceptionHelper;

        public PostAsyncTests()
        {
            _helper = new UsersAccessorHelper();
            _exceptionHelper = new UsersAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Add_User_To_Accessor()
        {
            // Arrange
            var postUser = _helper.GeneratePostUser();
            postUser.GameId = null!;

            // Act
            var returnedUser = await _helper.Accessor.PostAsync(postUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.Created);
                returnedUser.Data.Should().NotBeNull();
                returnedUser.Data.Should().BeEquivalentTo(postUser,
                    options => options
                        .Excluding(u => u.HashData)
                        .Excluding(u => u.GameId)
                );
                returnedUser.Data!.Hash.Should().Be(postUser.HashData.Hash);
                returnedUser.Data!.Salt.Should().Be(postUser.HashData.Salt);
                returnedUser.Data!.UserGame.Should().BeNull();

                // Check the database.
                var storedUser = await _helper.GetUser(returnedUser.Data!.Id);
                storedUser.Should().NotBeNull();
                storedUser.Should().BeEquivalentTo(postUser,
                    options => options
                        .Excluding(u => u.HashData)
                        .Excluding(u => u.GameId)
                );
                storedUser.Hash.Should().Be(postUser.HashData.Hash);
                storedUser.Salt.Should().Be(postUser.HashData.Salt);
                storedUser.UserGame.Should().BeNull();

                storedUser.Should().BeEquivalentTo(returnedUser.Data);
            }
        }

        [Fact]
        public async Task Can_Add_User_With_Game_Assignment_To_Accessor()
        {
            // Arrange
            var storedGame = await _helper.AddGame();
            var postUser = _helper.GeneratePostUser();
            postUser.GameId = storedGame.Id;

            // Act
            var returnedUser = await _helper.Accessor.PostAsync(postUser);

            // Assert
            using (new AssertionScope())
            {
                // Check the response.
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.Created);
                returnedUser.Data.Should().NotBeNull();
                returnedUser.Data.Should().BeEquivalentTo(postUser,
                    options => options
                        .Excluding(u => u.HashData)
                        .Excluding(u => u.GameId)
                );
                returnedUser.Data!.Hash.Should().Be(postUser.HashData.Hash);
                returnedUser.Data!.Salt.Should().Be(postUser.HashData.Salt);
                returnedUser.Data!.UserGame!.GameId.Should().Be(postUser.GameId.Value);

                // Check the database.
                var storedUser = await _helper.GetUser(returnedUser.Data!.Id);
                storedUser.Should().NotBeNull();
                storedUser.Should().BeEquivalentTo(postUser,
                    options => options
                        .Excluding(u => u.HashData)
                        .Excluding(u => u.GameId)
                );
                storedUser.Hash.Should().Be(postUser.HashData.Hash);
                storedUser.Salt.Should().Be(postUser.HashData.Salt);
                storedUser.UserGame!.GameId.Should().Be(postUser.GameId.Value);
                storedUser.Should().BeEquivalentTo(returnedUser.Data);
            }
        }

        [Fact]
        public async Task Can_Not_Add_User_To_Accessor_If_Assigned_Game_Does_Not_Exist()
        {
            // Arrange
            var postUser = _helper.GeneratePostUser();

            // Act
            var returnedUser = await _helper.Accessor.PostAsync(postUser);

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
        public async Task Can_Not_Add_User_To_Accessor_If_Game_Is_Already_Assigned()
        {
            // Arrange
            var storedUser = await _helper.AddUser();
            var postUser = _helper.GeneratePostUser();
            postUser.GameId = storedUser.UserGame!.GameId;

            // Act
            var returnedUser = await _helper.Accessor.PostAsync(postUser);

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
        public async Task Can_Not_Add_User_To_Accessor_If_User_Name_Already_Exists()
        {
            // Arrange
            var storedUser = await _helper.AddUser();
            var postUser = _helper.GeneratePostUser();
            postUser.UserName = storedUser.UserName;

            // Act
            var returnedUser = await _helper.Accessor.PostAsync(postUser);

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
            var postUser = _helper.GeneratePostUser();

            // Act
            Func<Task<IDataResponse<User?>>> func = () => _exceptionHelper.Accessor.PostAsync(postUser);

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
