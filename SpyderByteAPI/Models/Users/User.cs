using SpyderByteResources.Enums;

namespace SpyderByteAPI.Models.Users
{
    public record User
    {
        public Guid Id { get; init; }

        public string UserName { get; init; } = string.Empty;

        public UserType UserType { get; init; } = UserType.Restricted;

        public Guid? GameId { get; init; }
    }
}
