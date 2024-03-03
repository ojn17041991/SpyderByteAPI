using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Authentication;

namespace SpyderByteAPI.Models.Users
{
    public class PostHashedUser
    {
        public string UserName { get; set; } = string.Empty;

        public HashData HashData { get; set; } = new HashData();

        public UserType UserType{ get; set; }

        public Guid? GameId { get; set; }
    }
}
