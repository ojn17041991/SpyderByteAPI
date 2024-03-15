using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SpyderByteResources.Enums;
using System.IdentityModel.Tokens.Jwt;

namespace SpyderByteResources.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetToken(this HttpContext? httpContext)
        {
            if (httpContext?.Request?.Headers == null) return string.Empty;

            var bearerToken = httpContext.Request.Headers["Authorization"].ToString();
            if (bearerToken.IsNullOrEmpty()) return string.Empty;

            var token = bearerToken.Split(' ').LastOrDefault();
            if (token.IsNullOrEmpty()) return string.Empty;

            return token!;
        }

        public static Guid GetLoggedInUserId(this HttpContext? httpContext)
        {
            var token = httpContext.GetToken();
            if (token == null) return Guid.Empty;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.SingleOrDefault(c => c.Type == ClaimType.UserId.ToDescription())!.Value;
            return Guid.Parse(userIdClaim);
        }
    }
}
