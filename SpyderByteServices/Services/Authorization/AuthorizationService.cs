using Microsoft.Extensions.Logging;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using SpyderByteResources.Responses;
using SpyderByteServices.Services.Authorization.Abstract;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteDataAccess.Accessors.Games.Abstract;

namespace SpyderByteServices.Services.Authorization
{
    public class AuthorizationService(IUsersAccessor usersAccessor, IGamesAccessor gamesAccessor, ILogger<AuthorizationService> logger) : IAuthorizationService
    {
        private readonly IUsersAccessor usersAccessor = usersAccessor;
        private readonly IGamesAccessor gamesAccessor = gamesAccessor;
        private readonly ILogger<AuthorizationService> logger = logger;

        public async Task<IDataResponse<bool>> UserHasAccessToGame(Guid userId, Guid gameId)
        {
            var userResponse = await usersAccessor.GetAsync(userId);
            if (userResponse.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to check access for user {userId}. Could not find user in database.");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            var user = userResponse.Data!;
            if (user.UserType == UserType.Admin)
            {
                return new DataResponse<bool>(true, ModelResult.OK);
            }

            if (user.UserGame == null)
            {
                logger.LogInformation($"Access denied for user {userId} to leaderboard {gameId}");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            if (user.UserGame.GameId != gameId)
            {
                logger.LogInformation($"Access denied for user {userId} to game {gameId}");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            return new DataResponse<bool>(true, ModelResult.OK);
        }

        public async Task<IDataResponse<bool>> UserHasAccessToLeaderboard(Guid userId, Guid leaderboardId)
        {
            var userResponse = await usersAccessor.GetAsync(userId);
            if (userResponse.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to check access for user {userId}. Could not find user in database.");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            var user = userResponse.Data!;
            if (user.UserType == UserType.Admin)
            {
                return new DataResponse<bool>(true, ModelResult.OK);
            }

            if (user.UserGame == null)
            {
                logger.LogInformation($"Access denied for user {userId} to leaderboard {leaderboardId}");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            var gameResponse = await gamesAccessor.GetSingleAsync(user.UserGame.GameId);
            if (gameResponse.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to check access for user {userId}. Could not find user game in database.");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            var game = gameResponse.Data!;
            if (game.LeaderboardGame == null)
            {
                logger.LogInformation($"Access denied for user {userId} to leaderboard {leaderboardId}");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }

            if (game.LeaderboardGame.LeaderboardId != leaderboardId)
            {
                logger.LogInformation($"Access denied for user {userId} to leaderboard {leaderboardId}");
                return new DataResponse<bool>(false, ModelResult.Unauthorized);
            }
            
            return new DataResponse<bool>(true, ModelResult.OK);
        }
    }
}
