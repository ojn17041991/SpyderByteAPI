using Microsoft.IdentityModel.Tokens;

namespace SpyderByteAPI.Helpers
{
    public static class AuthenticationHelper
    {
        // OJN: Could have faster lookups with HashSet.

        private static int MAX_TOKENS_STORED = 100;
        private static string[] blacklist = new string[MAX_TOKENS_STORED];
        private static int blacklistIndex = 0;

        public static void AddTokenToBlacklist(string token)
        {
            blacklist[blacklistIndex] = token;
            blacklistIndex = ++blacklistIndex % MAX_TOKENS_STORED;
        }

        public static bool IsTokenBlacklisted(string token)
        {
            return blacklist.Contains(token);
        }

        public static string GetTokenFromHttpContext(HttpContext? httpContext)
        {
            if (httpContext?.Request?.Headers == null) return string.Empty;

            var bearerToken = httpContext.Request.Headers["Authorization"].ToString();
            if (bearerToken.IsNullOrEmpty()) return string.Empty;

            var token = bearerToken.Split(' ').LastOrDefault();
            if (token.IsNullOrEmpty()) return string.Empty;

            return token!;
        }
    }
}
