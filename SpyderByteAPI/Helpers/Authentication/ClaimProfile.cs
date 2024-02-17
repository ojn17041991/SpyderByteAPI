using SpyderByteAPI.Enums;
using SpyderByteAPI.Extensions;
using System.Security.Claims;

namespace SpyderByteAPI.Helpers.Authentication
{
    public class ClaimProfile
    {
        private const string claimEnabled = "true";

        public static IEnumerable<Claim> AdministratorClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimType.WriteGames.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteJams.ToDescription(), claimEnabled),
                new Claim(ClaimType.WriteLeaderboards.ToDescription(), claimEnabled),
                new Claim(ClaimType.DeleteLeaderboards.ToDescription(), claimEnabled)
            };
        }

        public static IEnumerable<Claim> RestrictedClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimType.WriteLeaderboards.ToDescription(), claimEnabled)
            };
        }

        public static IEnumerable<Claim> UtilityClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimType.DataBackup.ToDescription(), claimEnabled)
            };
        }
    }
}
