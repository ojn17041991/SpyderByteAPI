﻿using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteTest.DataAccess.UsersAccessorTests.Helpers;

namespace SpyderByteTest.DataAccess.UsersAccessorTests
{
    public class GetAsyncTests
    {
        private readonly UsersAccessorHelper _helper;
        private readonly UsersAccessorExceptionHelper _exceptionHelper;

        public GetAsyncTests()
        {
            _helper = new UsersAccessorHelper();
            _exceptionHelper = new UsersAccessorExceptionHelper();
        }

        [Fact]
        public async Task Can_Get_User_Record_By_Id_From_Accessor()
        {
            // Arrange
            var storedUser = await _helper.AddUser();

            // Act
            var returnedUser = await _helper.Accessor.GetAsync(storedUser.Id);

            // Assert
            using (new AssertionScope())
            {
                returnedUser.Should().NotBeNull();
                returnedUser.Result.Should().Be(ModelResult.OK);
                returnedUser.Data.Should().NotBeNull();
                returnedUser.Data.Should().BeEquivalentTo
                (
                    storedUser,
                    options => options
                        .Excluding(u => u.UserGame)
                );
                returnedUser.Data!.UserGame.Should().BeEquivalentTo
                (
                    storedUser.UserGame,
                    options => options
                        .Excluding(ug => ug!.Game)
                        .Excluding(ug => ug!.User)
                );
            }
        }

        [Fact]
        public async Task Exceptions_Are_Caught_And_Handled()
        {
            // Arrange

            // Act
            Func<Task<IDataResponse<User?>>> func = () => _exceptionHelper.Accessor.GetAsync(Guid.NewGuid());

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
