using SpyderByteServices.Models.Games;

namespace SpyderByteServices.Models.Leaderboards
{
    public record LeaderboardGame
    {
        public int Id { get; init; }

        public Guid GameId { get; init; }

        public Game Game { get; init; } = new Game();

        public Guid LeaderboardId { get; init; }

        public Leaderboard Leaderboard { get; init; } = null!;
    }
}
