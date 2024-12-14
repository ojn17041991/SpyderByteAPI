using Microsoft.AspNetCore.Http;
using SpyderByteResources.Models.Responses.Abstract;
using SpyderByteServices.Models.Authentication;

namespace SpyderByteServices.Services.Authentication.Abstract
{
    public interface IAuthenticationService
    {
        Task<IDataResponse<string>> AuthenticateAsync(Login login);

        IDataResponse<string> Refresh(HttpContext context);

        IDataResponse<string> Deauthenticate(HttpContext context);
    }
}
