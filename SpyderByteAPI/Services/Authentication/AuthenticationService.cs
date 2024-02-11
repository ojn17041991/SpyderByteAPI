using Microsoft.IdentityModel.Tokens;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authentication;
using SpyderByteAPI.Helpers.Authorization;
using SpyderByteAPI.Models.Auth;
using SpyderByteAPI.Services.Auth.Abstract;
using System.Security.Claims;

namespace SpyderByteAPI.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> logger;
        private readonly IConfiguration configuration;
        private readonly TokenEncoder tokenEncoder;

        public AuthenticationService(ILogger<AuthenticationService> logger, IConfiguration configuration, TokenEncoder tokenEncoder)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.tokenEncoder = tokenEncoder;
        }

        public IDataResponse<string> Authenticate(Authentication login)
        {
            IEnumerable<Claim> claims;

            if (login.UserName == configuration["Authentication:Administrator:UserName"] &&
                login.Secret == configuration["Authentication:Administrator:Secret"])
            {
                claims = ClaimProfile.AdministratorClaims();
            }
            else if (login.UserName == configuration["Authentication:Restricted:UserName"] &&
                     login.Secret == configuration["Authentication:Restricted:Secret"])
            {
                claims = ClaimProfile.RestrictedClaims();
            }
            else if (login.UserName == configuration["Authentication:Utility:UserName"] &&
                     login.Secret == configuration["Authentication:Utility:Secret"])
            {
                claims = ClaimProfile.UtilityClaims();
            }
            else
            {
                logger.LogError($"Failed to authenticate {login.UserName} user. Invalid credentials.");
                return new DataResponse<string>(string.Empty, ModelResult.Unauthorized);
            }

            var token = tokenEncoder.Encode(claims);
            if (token.IsNullOrEmpty())
            {
                logger.LogError($"Failed to authenticate {login.UserName} user. Unable to generate token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            logger.LogInformation($"Authenticated {login.UserName} user.");
            return new DataResponse<string>(token, ModelResult.OK);
        }

        public IDataResponse<string> Deauthenticate(HttpContext context)
        {
            var token = TokenExtractor.GetTokenFromHttpContext(context);
            if (token.IsNullOrEmpty())
            {
                logger.LogError($"Failed to deauthenticate user. Unable to find authorization token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            TokenBlacklister.AddTokenToBlacklist(token);
            return new DataResponse<string>(token, ModelResult.OK);
        }

        public IDataResponse<string> Refresh(HttpContext context)
        {
            var token = TokenExtractor.GetTokenFromHttpContext(context);
            if (token.IsNullOrEmpty())
            {
                logger.LogError($"Failed to refresh token. Unable to find authorization token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            var claims = tokenEncoder.Decode(token);
            if (claims.IsNullOrEmpty())
            {
                logger.LogError($"Failed to refresh token. Unable to extract claims from token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            var refreshToken = tokenEncoder.Encode(claims);
            if (refreshToken.IsNullOrEmpty())
            {
                logger.LogError($"Failed to refresh token. Unable to generate token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            TokenBlacklister.AddTokenToBlacklist(token);
            return new DataResponse<string>(refreshToken, ModelResult.OK);
        }
    }
}
