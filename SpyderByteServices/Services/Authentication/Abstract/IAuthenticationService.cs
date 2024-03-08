using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models.Authentication;

namespace SpyderByteAPI.Services.Auth.Abstract
{
    public interface IAuthenticationService
    {
        Task<IDataResponse<string>> AuthenticateAsync(Login login);

        IDataResponse<string> Refresh(HttpContext context);

        IDataResponse<string> Deauthenticate(HttpContext context);
    }
}
