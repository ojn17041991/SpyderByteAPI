using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using SpyderByteResources.Responses;
using SpyderByteServices.Services.Password.Abstract;
using SpyderByteServices.Services.Users;
using SpyderByteServices.Models.Users;
using SpyderByteServices.Models.Authentication;
using Microsoft.FeatureManagement;

namespace SpyderByteTest.Services.UsersServiceTests.Helpers
{
    public class UsersServiceHelper
    {
        public UsersService Service;

        private readonly Fixture _fixture;
        private readonly IMapper _mapper;
        private readonly IList<SpyderByteDataAccess.Models.Users.User> _users;

        public UsersServiceHelper()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _users = new List<SpyderByteDataAccess.Models.Users.User>();

            var usersAccessor = new Mock<IUsersAccessor>();
            usersAccessor.Setup(x =>
                x.GetAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var user = _users.SingleOrDefault(u => u.Id == id);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        _users.SingleOrDefault(u => u.Id == id),
                        user == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });
            usersAccessor.Setup(x =>
                x.GetByUserNameAsync(
                    It.IsAny<string>()
            )).Returns((string userName) =>
            {
                var user = _users.SingleOrDefault(u => u.UserName == userName);
                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        user,
                        user == null ? ModelResult.NotFound : ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });
            usersAccessor.Setup(x =>
                x.PostAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Users.PostUser>()
            )).Returns((SpyderByteDataAccess.Models.Users.PostUser postUser) =>
            {
                var user = _fixture.Create<SpyderByteDataAccess.Models.Users.User>();
                user.UserName = postUser.UserName;
                user.UserType = postUser.UserType;
                _users.Add(user);

                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        user,
                        ModelResult.Created
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });
            usersAccessor.Setup(x =>
                x.PatchAsync(
                    It.IsAny<SpyderByteDataAccess.Models.Users.PatchUser>()
            )).Returns((SpyderByteDataAccess.Models.Users.PatchUser patchUser) =>
            {
                var user = _users.Single(u => u.Id == patchUser.Id);
                user.UserGame!.GameId = patchUser.GameId!.Value;

                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        user,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });
            usersAccessor.Setup(x =>
                x.DeleteAsync(
                    It.IsAny<Guid>()
            )).Returns((Guid id) =>
            {
                var user = _users.Single(u => u.Id == id);
                _users.Remove(user);

                return Task.FromResult(
                    new DataResponse<SpyderByteDataAccess.Models.Users.User?>(
                        user,
                        ModelResult.OK
                    )
                    as IDataResponse<SpyderByteDataAccess.Models.Users.User?>
                );
            });

            var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<SpyderByteServices.Mappers.MapperProfile>());
            _mapper = new Mapper(mapperConfiguration);

            var logger = new Mock<ILogger<UsersService>>();

            var passwordService = new Mock<IPasswordService>();
            passwordService.Setup(x =>
                x.GenerateNewHash(
                    It.IsAny<string>()
            )).Returns((string password) =>
            {
                return new HashData
                {
                    Hash = Guid.NewGuid().ToString(),
                    Salt = Guid.NewGuid().ToString(),
                    Pepper = 'x'
                };
            });

            var featureManager = new Mock<IFeatureManager>();
            featureManager.Setup(x =>
                x.IsEnabledAsync(
                    It.IsAny<string>()
            )).Returns((string feature) =>
                Task.FromResult(false)
            );

            Service = new UsersService(usersAccessor.Object, _mapper, logger.Object, passwordService.Object, featureManager.Object);
        }

        public SpyderByteDataAccess.Models.Users.User AddUser(UserType userType)
        {
            var user = _fixture.Create<SpyderByteDataAccess.Models.Users.User>();
            user.UserType = userType;
            _users.Add(user);
            return user;
        }

        public PostUser GeneratePostUser()
        {
            return _fixture.Create<PostUser>();
        }

        public PatchUser GeneratePatchUser()
        {
            return _fixture.Create<PatchUser>();
        }
    }
}
