using Microsoft.IdentityModel.Tokens;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers;
using SpyderByteAPI.Models.Auth;
using SpyderByteAPI.Services.Auth.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpyderByteAPI.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // OJN: This whole thing probably needs rethinking.
        // Users should be stored in the database, but the database is tied to the Web App. It should be independent...
        // Also, should there be a user per game to limit the amount of damage they can do?

        public IDataResponse<string> Authenticate(Authentication login)
        {
            IEnumerable<Claim> claims = new List<Claim>();

            if (login.UserName == configuration["Authentication:Administrator:UserName"] &&
                login.Secret == configuration["Authentication:Administrator:Secret"])
            {
                claims = getAdministratorClaims();
            }
            else if (login.UserName == configuration["Authentication:Restricted:UserName"] &&
                     login.Secret == configuration["Authentication:Restricted:Secret"])
            {
                claims = getRestrictedClaims();
            }
            else
            {
                return new DataResponse<string>(string.Empty, ModelResult.Unauthorized);
            }

            var token = encode(claims);
            return new DataResponse<string>(token, ModelResult.OK);
        }

        public IDataResponse<bool> Deauthenticate(HttpContext context)
        {
            // OJN: Bool is returned for no reason. I just need something as T

            var token = AuthenticationHelper.GetTokenFromHttpContext(context);
            if (token.IsNullOrEmpty()) return new DataResponse<bool>(false, ModelResult.Error); // OJN: Is Error correct here?

            AuthenticationHelper.AddTokenToBlacklist(token);
            return new DataResponse<bool>(true, ModelResult.OK);
        }

        public IDataResponse<string> Refresh(HttpContext context)
        {
            var token = AuthenticationHelper.GetTokenFromHttpContext(context);
            if (token.IsNullOrEmpty()) return new DataResponse<string>(string.Empty, ModelResult.Error); // OJN: Is Error correct here?

            var claims = decode(token);
            if (claims.IsNullOrEmpty()) return new DataResponse<string>(string.Empty, ModelResult.Error); // OJN: Is Error correct here?

            var refreshToken = encode(claims);
            if (refreshToken.IsNullOrEmpty()) return new DataResponse<string>(string.Empty, ModelResult.Error); // OJN: Is Error correct here?

            AuthenticationHelper.AddTokenToBlacklist(token);
            return new DataResponse<string>(refreshToken, ModelResult.OK);
        }

        private string encode(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:EncodingKey"] ?? string.Empty));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["Authentication:TimeoutMinutes"])),
                Issuer = configuration["Authentication:Issuer"],
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        private IEnumerable<Claim> decode(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return claims.ToList();
        }

        private IEnumerable<Claim> getAdministratorClaims()
        {
            return new List<Claim>
            {
                new Claim("ReadGames", true.ToString()),
                new Claim("WriteGames", true.ToString()),
                new Claim("ReadJams", true.ToString()),
                new Claim("WriteJams", true.ToString()),
                new Claim("ReadLeadboards", true.ToString()),
                new Claim("WriteLeaderboards", true.ToString())
            };
        }

        private IEnumerable<Claim> getRestrictedClaims()
        {
            return new List<Claim>
            {
                // OJN: Later on this will need to be limited to a specific game or possibly set of games.
                new Claim("ReadGames", true.ToString()),
                new Claim("ReadJams", true.ToString()),
                new Claim("ReadLeadboards", true.ToString()),
                new Claim("WriteLeaderboards", true.ToString())
            };
        }
    }
}
