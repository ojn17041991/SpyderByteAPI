using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Authentication;

namespace SpyderByteAPI.Models.Users
{
    public class PostHashedUser
    {
        public string Id { get; set; } = string.Empty;

        public HashData HashData { get; set; } = new HashData();

        public UserType UserType{ get; set; }
    }
}
