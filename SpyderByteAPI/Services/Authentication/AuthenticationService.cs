using Microsoft.IdentityModel.Tokens;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers.Authentication;
using SpyderByteAPI.Helpers.Authorization;
using SpyderByteAPI.Models.Authentication;
using SpyderByteAPI.Services.Auth.Abstract;
using System.Security.Claims;

namespace SpyderByteAPI.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUsersAccessor usersAccessor;
        private readonly ILogger<AuthenticationService> logger;
        private readonly TokenEncoder tokenEncoder;
        private readonly PasswordHasher passwordHasher;

        public AuthenticationService(IUsersAccessor usersAccessor, ILogger<AuthenticationService> logger, TokenEncoder tokenEncoder, PasswordHasher passwordHasher)
        {
            this.usersAccessor = usersAccessor;
            this.logger = logger;
            this.tokenEncoder = tokenEncoder;
            this.passwordHasher = passwordHasher;
        }

        public async Task<IDataResponse<string>> AuthenticateAsync(Login login)
        {
            IEnumerable<Claim> claims;

            var userResponse = await usersAccessor.GetAsync(login.UserName);
            var user = userResponse.Data!;

            var passwordVerification = new PasswordVerification
            {
                Password = login.Password,
                Hash = user.Hash,
                Salt = user.Salt
            };

            bool passwordIsValid = passwordHasher.IsPasswordValid(passwordVerification);
            if (passwordIsValid == false) { return new DataResponse<string>(string.Empty, ModelResult.Unauthorized); }

            // OJN: Get claims based on UserType.
                claims = ClaimProfile.AdministratorClaims();

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
