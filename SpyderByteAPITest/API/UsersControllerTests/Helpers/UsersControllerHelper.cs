using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using SpyderByteAPI.Controllers;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;
using SpyderByteResources.Responses;
using SpyderByteServices.Services.Users.Abstract;
using SpyderByteAPI.Models.Users;

namespace SpyderByteTest.API.UsersControllerTests.Helpers
{
    public class UsersControllerHelper
    {
        public UsersController Controller;

        private readonly Fixture fixture;

        private ModelResult currentModelResult = ModelResult.OK;

        public UsersControllerHelper()
        {
            var usersService = new Mock<IUsersService>();
            usersService.Setup(x =>
                x.GetAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Users.User>(
                        fixture.Create<SpyderByteServices.Models.Users.User>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Users.User?>
                );
            });
            usersService.Setup(x =>
                x.PostAsync(
                    It.IsAny<SpyderByteServices.Models.Users.PostUser>()
                )
            ).Returns((SpyderByteServices.Models.Users.PostUser postUser) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Users.User>(
                        fixture.Create<SpyderByteServices.Models.Users.User>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Users.User?>
                );
            });
            usersService.Setup(x =>
                x.PatchAsync(
                    It.IsAny<SpyderByteServices.Models.Users.PatchUser>()
                )
            ).Returns((SpyderByteServices.Models.Users.PatchUser patchUser) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Users.User>(
                        fixture.Create<SpyderByteServices.Models.Users.User>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Users.User?>
                );
            });
            usersService.Setup(x =>
                x.DeleteAsync(
                    It.IsAny<Guid>()
                )
            ).Returns((Guid id) =>
            {
                return Task.FromResult(
                    new DataResponse<SpyderByteServices.Models.Users.User>(
                        fixture.Create<SpyderByteServices.Models.Users.User>(),
                        currentModelResult
                    )
                    as IDataResponse<SpyderByteServices.Models.Users.User?>
                );
            });

            var modelResources = new Mock<IStringLookup<ModelResult>>();
            modelResources.Setup(x =>
                x.GetResource(
                    It.IsAny<ModelResult>()
                )
            ).Returns((ModelResult modelResult) => {
                return string.Empty;
            });

            fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Customize<IFormFile>(f => f.FromFactory(() => new Mock<IFormFile>().Object));

            var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<SpyderByteAPI.Mappers.MapperProfile>());
            var mapper = new Mapper(mapperConfiguration);

            Controller = new UsersController(usersService.Object, mapper, modelResources.Object);
        }

        public void SetCurrentModelResult(ModelResult currentModelResult)
        {
            this.currentModelResult = currentModelResult;
        }

        public PostUser GeneratePostUser()
        {
            return fixture.Create<PostUser>();
        }

        public PatchUser GeneratePatchUser()
        {
            return fixture.Create<PatchUser>();
        }
    }
}
