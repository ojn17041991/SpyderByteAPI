namespace SpyderByteAPI.Models.Leaderboards
{
    public record LeaderboardRecord
    {
        public Guid Id { get; init; }

        public Guid LeaderboardId { get; init; }

        public string Player { get; init; } = string.Empty;

        public long Score { get; init; }

        public DateTime Timestamp { get; init; }
    }
}
