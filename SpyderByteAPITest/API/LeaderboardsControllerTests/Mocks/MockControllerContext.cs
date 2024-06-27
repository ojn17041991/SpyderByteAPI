using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SpyderByteResources.Enums;
using SpyderByteResources.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SpyderByteTest.API.LeaderboardsControllerTests.Mocks
{
    public class MockControllerContext : ControllerContext
    {
        private readonly Fixture fixture;

        public MockControllerContext()
        {
            fixture = new Fixture();

            var httpRequest = new Mock<HttpRequest>();
            httpRequest.Setup(x =>
                x.Headers
            ).Returns(
                new HeaderDictionary
                {
                    { "Authorization", generateToken() }
                }
            );

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x =>
                x.Request
            ).Returns(
                httpRequest.Object
            );

            HttpContext = httpContext.Object;
        }

        private string generateToken()
        {
            // OJN: Is this the best approach?

            var claims = new List<Claim>
            {
                new Claim(ClaimType.UserId.ToDescription(), Guid.NewGuid().ToString()),
            };

            var secretKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(fixture.Create<string>()));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            var tokenString = handler.WriteToken(token);
            return tokenString;
        }
    }
}
