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

            // Make sure the user exists before attempting to authenticate.
            var response = await usersAccessor.GetAsync(login.UserName);
            if (response.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to authenticate user {login.UserName}. Could not find user in database.");
                return new DataResponse<string>(string.Empty, ModelResult.Unauthorized);
            }

            // Build the password verification object from the database record.
            var user = response.Data!;
            var passwordVerification = new PasswordVerification
            {
                Password = login.Password,
                Hash = user.Hash,
                Salt = user.Salt
            };

            // Check if the password is correct for the user.
            bool passwordIsValid = passwordHasher.IsPasswordValid(passwordVerification);
            if (passwordIsValid == false)
            {
                logger.LogError($"Failed to authenticate user {login.UserName}. Password incorrect.");
                return new DataResponse<string>(string.Empty, ModelResult.Unauthorized);
            }

            // Get the user claims based on user type.
            switch (user.UserType)
            {
                case UserType.Admin:
                    claims = ClaimProfile.AdministratorClaims();
                    break;
                case UserType.Restricted:
                    claims = ClaimProfile.RestrictedClaims();
                    break;
                case UserType.Utility:
                    claims = ClaimProfile.UtilityClaims();
                    break;
                default:
                    logger.LogError($"Failed to authenticate user. User Type {(int)user.UserType} not recognised.");
                    return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            // Encode the claims to produce the token.
            var token = tokenEncoder.Encode(claims);
            if (token.IsNullOrEmpty())
            {
                logger.LogError($"Failed to authenticate {login.UserName} user. Unable to generate token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            // Login successful. Return token.
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
