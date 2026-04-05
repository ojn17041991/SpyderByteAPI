using SpyderByteResources.Enums;

namespace SpyderByteServices.Models.Users
{
    public record User
    {
        public Guid Id { get; init; }

        public string UserName { get; init; } = string.Empty;

        public string Hash { get; init; } = string.Empty;

        public string Salt { get; init; } = string.Empty;

        public UserType UserType { get; init; } = UserType.Restricted;

        public UserGame? UserGame { get; init; } = null!;
    }
}
