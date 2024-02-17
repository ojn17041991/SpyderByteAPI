using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpyderByteAPI.Helpers.Authentication
{
    public class TokenEncoder
    {
        private readonly IConfiguration configuration;

        public TokenEncoder(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Encode(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:EncodingKey"] ?? string.Empty));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["Authentication:TimeoutMinutes"])),
                Issuer = configuration["Authentication:Issuer"],
                Audience = configuration["Authentication:Audience"],
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public IEnumerable<Claim> Decode(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return claims.ToList();
        }
    }
}
