using SpyderByteResources.Enums;

namespace SpyderByteServices.Models.Users
{
    public class PostUser
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserType UserType { get; set; } = UserType.Restricted;

        public Guid? GameId { get; set; }
    }
}
