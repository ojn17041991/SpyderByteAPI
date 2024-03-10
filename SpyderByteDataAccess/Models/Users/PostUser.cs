using SpyderByteDataAccess.Models.Authentication;
using SpyderByteResources.Enums;

namespace SpyderByteDataAccess.Models.Users
{
    public class PostUser
    {
        public string UserName { get; set; } = string.Empty;

        public HashData HashData { get; set; } = new HashData();

        public UserType UserType { get; set; }

        public Guid? GameId { get; set; }
    }
}
