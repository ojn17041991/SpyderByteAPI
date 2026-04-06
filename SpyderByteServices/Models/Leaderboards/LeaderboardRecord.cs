namespace SpyderByteServices.Models.Leaderboards
{
    public record LeaderboardRecord
    {
        public Guid Id { get; init; }

        public Guid LeaderboardId { get; init; }

        public Leaderboard Leaderboard { get; init; } = null!;

        public string Player { get; init; } = string.Empty;

        public long Score { get; init; }

        public DateTime Timestamp { get; init; }
    }
}
