using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Moq;
using SpyderByteAPI.Controllers;
using SpyderByteAPI.Models.Authentication;
using SpyderByteResources.Enums;
using SpyderByteResources.Flags;
using SpyderByteResources.Responses;
using SpyderByteResources.Responses.Abstract;
using SpyderByteServices.Services.Authentication.Abstract;

namespace SpyderByteTest.API.AuthenticationControllerTests.Helpers
{
    public class AuthenticationControllerHelper
    {
        public AuthenticationController Controller;

        private readonly Fixture fixture;

        private bool allowAuthenticationRefresh = true;
        private ModelResult currentModelResult = ModelResult.OK;

        public AuthenticationControllerHelper()
        {
            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService.Setup(x =>
                x.AuthenticateAsync(
                    It.IsAny<SpyderByteServices.Models.Authentication.Login>()
                )
            ).Returns((SpyderByteServices.Models.Authentication.Login login) =>
            {
                return Task.FromResult(
                    new DataResponse<string>(
                        string.Empty,
                        currentModelResult
                    )
                    as IDataResponse<string>
                );
            });
            authenticationService.Setup(x =>
                x.Deauthenticate(
                    It.IsAny<HttpContext>()
                )
            ).Returns((HttpContext httpContext) =>
            {
                return new DataResponse<string>(
                    string.Empty,
                    currentModelResult
                )
                as IDataResponse<string>;
            });
            authenticationService.Setup(x =>
                x.Refresh(
                    It.IsAny<HttpContext>()
                )
            ).Returns((HttpContext httpContext) =>
            {
                return new DataResponse<string>(
                    string.Empty,
                    currentModelResult
                )
                as IDataResponse<string>;
            });

            fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var mapperConfiguration = new MapperConfiguration(config => config.AddProfile<SpyderByteAPI.Mappers.MapperProfile>());
            var mapper = new Mapper(mapperConfiguration);

            var featureManager = new Mock<IFeatureManager>();
            featureManager.Setup(x =>
                x.IsEnabledAsync(
                    It.IsAny<string>()
                )
            ).Returns((string featureName) =>
            {
                if (featureName == FeatureFlags.AllowAuthenticationRefresh)
                {
                    return Task.FromResult(allowAuthenticationRefresh);
                }
                return Task.FromResult(false);
            });

            Controller = new AuthenticationController(authenticationService.Object, mapper, featureManager.Object);
        }

        public void SetAllowAuthenticationRefresh(bool allowAuthenticationRefresh)
        {
            this.allowAuthenticationRefresh = allowAuthenticationRefresh;
        }

        public void SetCurrentModelResult(ModelResult currentModelResult)
        {
            this.currentModelResult = currentModelResult;
        }

        public Login GenerateLogin()
        {
            return fixture.Create<Login>();
        }
    }
}
