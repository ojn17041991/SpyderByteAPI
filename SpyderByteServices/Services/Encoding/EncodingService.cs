using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpyderByteServices.Services.Encoding.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SpyderByteServices.Services.Encoding
{
    public class EncodingService(IConfiguration configuration) : IEncodingService
    {
        private readonly IConfiguration configuration = configuration;

        public string Encode(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Authentication:EncodingKey"] ?? string.Empty));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["Authentication:TimeoutMinutes"])),
                Issuer = configuration["Authentication:Issuer"],
                Audience = configuration["Authentication:Audience"],
                SigningCredentials = signingCredentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            var tokenString = handler.WriteToken(token);
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
