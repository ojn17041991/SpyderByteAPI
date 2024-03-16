using FluentAssertions;
using SpyderByteResources.Enums;
using SpyderByteTest.Services.AuthorizationServiceTests.Helpers;

namespace SpyderByteTest.Services.AuthorizationServiceTests
{
    public class UserHasAccessToGameTests
    {
        private readonly AuthorizationServiceHelper _helper;

        public UserHasAccessToGameTests()
        {
            _helper = new AuthorizationServiceHelper();
        }

        [Fact]
        public async Task Can_Authorize_User_To_Access_Game()
        {
            // Arrange
            var storedGame = _helper.AddGame();
            var storedUser = _helper.AddUser(UserType.Restricted);
            _helper.AssociateUserWithGame(storedUser, storedGame);

            // Act
            var returnedGame = await _helper.Service.UserHasAccessToGame(storedUser.Id, storedGame.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.OK);
            returnedGame.Data!.Should().BeTrue();
        }

        [Fact]
        public async Task Can_Authorize_Admin_Without_Game_Assignment()
        {
            // Arrange
            var storedUser = _helper.AddUser(UserType.Admin);

            // Act
            var returnedGame = await _helper.Service.UserHasAccessToGame(storedUser.Id, Guid.NewGuid());

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.OK);
            returnedGame.Data!.Should().BeTrue();
        }

        [Fact]
        public async Task Can_Not_Authorize_User_To_Access_Unassigned_Game()
        {
            // Arrange
            var storedGame1 = _helper.AddGame();
            var storedGame2 = _helper.AddGame();
            var storedUser = _helper.AddUser(UserType.Restricted);
            _helper.AssociateUserWithGame(storedUser, storedGame1);

            // Act
            var returnedGame = await _helper.Service.UserHasAccessToGame(storedUser.Id, storedGame2.Id);

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.Unauthorized);
            returnedGame.Data!.Should().BeFalse();
        }

        [Fact]
        public async Task Can_Not_Authorize_User_Who_Does_Not_Exist()
        {
            // Arrange

            // Act
            var returnedGame = await _helper.Service.UserHasAccessToGame(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            returnedGame.Should().NotBeNull();
            returnedGame.Result.Should().Be(ModelResult.Unauthorized);
            returnedGame.Data!.Should().BeFalse();
        }
    }
}
