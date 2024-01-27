using Microsoft.IdentityModel.Tokens;

namespace SpyderByteAPI.Helpers.Authorization
{
    public class TokenExtractor
    {
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
