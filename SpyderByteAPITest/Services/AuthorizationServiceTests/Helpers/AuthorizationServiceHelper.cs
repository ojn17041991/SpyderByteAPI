using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using SpyderByteResources.Responses;
using SpyderByteServices.Services.Games;
using SpyderByteServices.Services.Authorization;
using SpyderByteDataAccess.Accessors.Users.Abstract;

namespace SpyderByteTest.Services.AuthorizationServiceTests.Helpers
{
    public class AuthorizationServiceHelper
    {
        public AuthorizationService Service;

        private readonly Fixture _fixture;
        private readonly IList<SpyderByteDataAccess.Models.Games.Game> _games;
        private readonly IList<SpyderByteDataAccess.Models.Users.User> _users;

        public AuthorizationServiceHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _games = new List<SpyderByteDataAccess.Models.Games.Game>();
            _users = new List<SpyderByteDataAccess.Models.Users.User>();

            var gamesAccessor = new Mock<IGamesAccessor>();
            gamesAccessor.Setup(s =>
                s.GetSingleAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var game = _games.SingleOrDefault(g => g.Id == id);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Games.Game?>(
                        game,
                        game == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Games.Game?>
                );
            });

            var usersAccessor = new Mock<IUsersAccessor>();
            usersAccessor.Setup(s =>
                s.GetAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var user = _users.SingleOrDefault(u => u.Id == id);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        user,
                        user == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });

            var logger = new Mock<ILogger<AuthorizationService>>();

            Service = new AuthorizationService(usersAccessor.Object, gamesAccessor.Object, logger.Object);
        }

        public SpyderByteDataAccess.Models.Users.User AddUser(UserType userType)
        {
            var user = _fixture.Create<SpyderByteDataAccess.Models.Users.User>();
            user.UserType = userType;
            _users.Add(user);
            return user;
        }

        public SpyderByteDataAccess.Models.Games.Game AddGame()
        {
            var game = _fixture.Create<SpyderByteDataAccess.Models.Games.Game>();
            _games.Add(game);
            return game;
        }

        public void AssociateUserWithGame(SpyderByteDataAccess.Models.Users.User user, SpyderByteDataAccess.Models.Games.Game game)
        {
            _users.Single(u => u.Id == user.Id).UserGame!.Game = game;
            _users.Single(u => u.Id == user.Id).UserGame!.GameId = game.Id;
            _games.Single(g => g.Id == game.Id).UserGame!.User = user;
            _games.Single(g => g.Id == game.Id).UserGame!.UserId = user.Id;
        }
    }
}
