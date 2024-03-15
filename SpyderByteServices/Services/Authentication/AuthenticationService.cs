using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Extensions;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Helpers.Authentication;
using SpyderByteServices.Models.Authentication;
using SpyderByteServices.Services.Authentication.Abstract;
using System.Security.Claims;

namespace SpyderByteServices.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUsersAccessor usersAccessor;
        private readonly IMapper mapper;
        private readonly ILogger<AuthenticationService> logger;
        private readonly TokenEncoder tokenEncoder;
        private readonly PasswordHasher passwordHasher;

        public AuthenticationService(IUsersAccessor usersAccessor, IMapper mapper, ILogger<AuthenticationService> logger, TokenEncoder tokenEncoder, PasswordHasher passwordHasher)
        {
            this.usersAccessor = usersAccessor;
            this.mapper = mapper;
            this.logger = logger;
            this.tokenEncoder = tokenEncoder;
            this.passwordHasher = passwordHasher;
        }

        public async Task<IDataResponse<string>> AuthenticateAsync(Login login)
        {
            IEnumerable<Claim> claims;

            // Make sure the user exists before attempting to authenticate.
            var response = await usersAccessor.GetByUserNameAsync(login.UserName);
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

            // Force the thread to sleep to prevent timing-based attacks.
            int sleepTime = new Random().Next(500) + 500;
            Thread.Sleep(sleepTime);

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
