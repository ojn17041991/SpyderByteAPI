using SpyderByteAPI.Authorisation.Abstract;

namespace SpyderByteAPI.Authorisation
{
    public class UserSecretAccessor : ISecretAccessor
    {
        public string ApiKey { get; }

        public UserSecretAccessor(IConfiguration configuration)
        {
            ApiKey = configuration["SBAPIKEY"] ?? string.Empty;
        }
    }
}
