using SpyderByteServices.Models.Games;

namespace SpyderByteServices.Models.Users
{
    public record UserGame
    {
        public int Id { get; init; }

        public Guid UserId { get; init; }

        public User User { get; init; } = null!;

        public Guid GameId { get; init; }

        public Game Game { get; init; } = null!;
    }
}
