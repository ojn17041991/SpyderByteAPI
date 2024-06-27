using SpyderByteResources.Enums;

namespace SpyderByteAPI.Models.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public UserType UserType { get; set; } = UserType.Restricted;

        public Guid? GameId { get; set; }
    }
}
