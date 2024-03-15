using SpyderByteResources.Responses.Abstract;

namespace SpyderByteServices.Services.Authorization.Abstract
{
    public interface IAuthorizationService
    {
        Task<IDataResponse<bool>> UserHasAccessToGame(Guid userId, Guid gameId);

        Task<IDataResponse<bool>> UserHasAccessToLeaderboard(Guid userId, Guid leaderboardId);
    }
}
