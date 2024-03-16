using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;
using SpyderByteResources.Extensions;
using System.Security.Claims;

namespace SpyderByteServices.Helpers.Authentication
{
    public class ClaimProfile
    {
        private const string claimEnabled = "true";
        private const string globalAccess = "*";

        public static IEnumerable<Claim> AdministratorClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimType.UserId.ToDescription(), user.Id.ToString()),
                new Claim(ClaimType.ReadUsers.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteUsers.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteGames.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteJams.ToDescription(), claimEnabled),
                new Claim(ClaimType.ReadLeaderboards.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteLeaderboards.ToDescription(), claimEnabled),
                new Claim(ClaimType.DeleteLeaderboards.ToDescription(), claimEnabled),
                new Claim(ClaimType.AssignedGame.ToDescription(), globalAccess)
            };
        }

        public static IEnumerable<Claim> RestrictedClaims(User user)
        {
            var claims =  new List<Claim>
            {
                new Claim(ClaimType.UserId.ToDescription(), user.Id.ToString()),
                new Claim(ClaimType.ReadLeaderboards.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteLeaderboards.ToDescription(), claimEnabled)
            };

            if (user.UserGame != null)
            {
                claims.Add(new Claim(ClaimType.AssignedGame.ToDescription(), user.UserGame.GameId.ToString()));
            }

            return claims;
        }

        public static IEnumerable<Claim> UtilityClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimType.UserId.ToDescription(), user.Id.ToString()),
                new Claim(ClaimType.DataBackup.ToDescription(), claimEnabled)
            };
        }
    }
}
