using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteResources.Enums;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteResources.Models.Responses;
using SpyderByteServices.Services.Authentication;
using AutoFixture;
using AutoMapper;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteServices.Services.Password.Abstract;
using SpyderByteServices.Models.Authentication;
using SpyderByteServices.Services.Encoding.Abstract;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SpyderByteTest.Services.AuthenticationServiceTests.Helpers
{
    public class AuthenticationServiceHelper
    {
        public AuthenticationService Service;

        private readonly Fixture fixture;
        private readonly IList<SpyderByteDataAccess.Models.Users.User> users;
        private readonly IList<Login> logins;
        private readonly Mapper mapper;

        private bool useEmptyEncodingToken = false;
        private bool useEmptyDecodingToken = false;
        private bool useEmptyBearerToken = false;

        public AuthenticationServiceHelper()
        {
            fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            users = new List<SpyderByteDataAccess.Models.Users.User>();
            logins = new List<Login>();

            var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<SpyderByteServices.Mappers.MapperProfile>());
            mapper = new Mapper(mapperConfiguration);

            var usersAccessor = new Mock<IUsersAccessor>();
            usersAccessor.Setup(x =>
                x.GetByUserNameAsync(
                    It.IsAny<string>()
                )
            ).Returns((string userName) =>
            {
                var user = users.SingleOrDefault(u => u.UserName == userName);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        user,
                        user == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });

            var logger = new Mock<ILogger<AuthenticationService>>();

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x =>
                x.Encode(
                    It.IsAny<IEnumerable<Claim>>()
                )
            ).Returns((IEnumerable<Claim> claims) =>
            {
                return useEmptyEncodingToken ?
                    string.Empty :
                    Guid.NewGuid().ToString();
            });
            encodingService.Setup(x =>
                x.Decode(
                    It.IsAny<string>()
                )
            ).Returns((string token) =>
            {
                return useEmptyDecodingToken ?
                    Enumerable.Empty<Claim>() :
                    new List<Claim>
                    {
                        new Claim("Name", "Value"),
                    };
            });

            var passwordService = new Mock<IPasswordService>();
            passwordService.Setup(x =>
                x.IsPasswordValid(
                    It.IsAny<PasswordVerification>()
                )
            ).Returns((PasswordVerification passwordVerification) =>
            {
                return logins!.Any(l => l.Password == passwordVerification.Password);
            });

            Service = new AuthenticationService(usersAccessor.Object, logger.Object, encodingService.Object, passwordService.Object);
        }

        public SpyderByteServices.Models.Authentication.Login AddLogin(UserType userType)
        {
            var user = fixture.Create<SpyderByteDataAccess.Models.Users.User>();
            user.UserType = userType;
            users.Add(user);

            var login = new Login
            {
                UserName = user.UserName,
                Password = Guid.NewGuid().ToString()
            };
            logins.Add(login);

            return login;
        }

        public HttpContext GenerateHttpContext()
        {
            var token = $"Bearer {Guid.NewGuid()}";

            var headers = new HeaderDictionary
            {
                {
                    "Authorization", useEmptyBearerToken ?
                        string.Empty :
                        token
                }
            };

            var httpRequest = new Mock<HttpRequest>();
            httpRequest.Setup(x =>
                x.Headers
            ).Returns(
                headers
            );

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x =>
                x.Request
            ).Returns(
                httpRequest.Object
            );

            return httpContext.Object;
        }

        public void SetUseEmptyEncodingToken(bool useEmptyEncodingToken)
        {
            this.useEmptyEncodingToken = useEmptyEncodingToken;
        }

        public void SetUseEmptyDecodingToken(bool useEmptyDecodingToken)
        {
            this.useEmptyDecodingToken = useEmptyDecodingToken;
        }

        public void SetUseEmptyBearerToken(bool useEmptyBearerToken)
        {
            this.useEmptyBearerToken = useEmptyBearerToken;
        }
    }
}
