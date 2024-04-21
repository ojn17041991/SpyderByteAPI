using SpyderByteServices.Models.Authentication;

namespace SpyderByteServices.Services.Password.Abstract
{
    public interface IPasswordService
    {
        public HashData GenerateNewHash(string password);

        public bool IsPasswordValid(PasswordVerification passwordVerification);
    }
}
