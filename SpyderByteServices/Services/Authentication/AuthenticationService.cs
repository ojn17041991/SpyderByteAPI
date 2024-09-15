using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Extensions;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteResources.Helpers.Encoding;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Helpers.Authentication;
using SpyderByteServices.Models.Authentication;
using SpyderByteServices.Services.Authentication.Abstract;
using SpyderByteServices.Services.Encoding.Abstract;
using SpyderByteServices.Services.Password.Abstract;
using System.Security.Claims;

namespace SpyderByteServices.Services.Authentication
{
    public class AuthenticationService(IUsersAccessor usersAccessor, ILogger<AuthenticationService> logger, IEncodingService encodingService, IPasswordService passwordService) : IAuthenticationService
    {
        private readonly IUsersAccessor usersAccessor = usersAccessor;
        private readonly ILogger<AuthenticationService> logger = logger;
        private readonly IEncodingService encodingService = encodingService;
        private readonly IPasswordService passwordService = passwordService;

        public async Task<IDataResponse<string>> AuthenticateAsync(Login login)
        {
            IEnumerable<Claim> claims;

            // Make sure the user exists before attempting to authenticate.
            var response = await usersAccessor.GetByUserNameAsync(login.UserName);
            if (response.Result != ModelResult.OK)
            {
                logger.LogError($"Failed to authenticate user {LogEncoder.Encode(login.UserName)}. Could not find user in database.");
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

            // Force the thread to sleep to prevent timing-based attacks.
            int sleepTime = new Random().Next(500) + 500;
            Thread.Sleep(sleepTime);

            // Check if the password is correct for the user.
            bool passwordIsValid = passwordService.IsPasswordValid(passwordVerification);
            if (passwordIsValid == false)
            {
                logger.LogError($"Failed to authenticate user {LogEncoder.Encode(login.UserName)}. Password incorrect.");
                return new DataResponse<string>(string.Empty, ModelResult.Unauthorized);
            }

            // Get the user claims based on user type.
            switch (user.UserType)
            {
                case UserType.Admin:
                    claims = ClaimProfile.AdministratorClaims(user);
                    break;
                case UserType.Restricted:
                    claims = ClaimProfile.RestrictedClaims(user);
                    break;
                case UserType.Utility:
                    claims = ClaimProfile.UtilityClaims(user);
                    break;
                default:
                    logger.LogError($"Failed to authenticate user. User Type {(int)user.UserType} not recognised.");
                    return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            // Encode the claims to produce the token.
            var token = encodingService.Encode(claims);
            if (token.IsNullOrEmpty())
            {
                logger.LogError($"Failed to authenticate {LogEncoder.Encode(login.UserName)} user. Unable to generate token.");
                return new DataResponse<string>(string.Empty, ModelResult.Unauthorized);
            }

            // Login successful. Return token.
            logger.LogInformation($"Authenticated {LogEncoder.Encode(login.UserName)} user.");
            return new DataResponse<string>(token, ModelResult.OK);
        }

        public IDataResponse<string> Deauthenticate(HttpContext context)
        {
            var token = context.GetToken();
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
            var token = context.GetToken();
            if (token.IsNullOrEmpty())
            {
                logger.LogError($"Failed to refresh token. Unable to find authorization token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            var claims = encodingService.Decode(token);
            if (claims.IsNullOrEmpty())
            {
                logger.LogError($"Failed to refresh token. Unable to extract claims from token.");
                return new DataResponse<string>(string.Empty, ModelResult.Error);
            }

            var refreshToken = encodingService.Encode(claims);
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
