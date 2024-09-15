using System.Security.Claims;

namespace SpyderByteServices.Services.Encoding.Abstract
{
    public interface IEncodingService
    {
        string Encode(IEnumerable<Claim> claims);

        IEnumerable<Claim> Decode(string token);
    }
}
