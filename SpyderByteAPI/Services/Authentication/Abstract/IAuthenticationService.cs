using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models.Auth;

namespace SpyderByteAPI.Services.Auth.Abstract
{
    public interface IAuthenticationService
    {
        IDataResponse<string> Authenticate(Authentication login);

        IDataResponse<string> Refresh(HttpContext context);

        IDataResponse<string> Deauthenticate(HttpContext context);
    }
}
