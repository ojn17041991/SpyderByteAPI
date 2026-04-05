using SpyderByteResources.Enums;

namespace SpyderByteServices.Models.Users
{
    public record PostUser
    {
        public string UserName { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public UserType UserType { get; init; } = UserType.Restricted;

        public Guid? GameId { get; init; }
    }
}
