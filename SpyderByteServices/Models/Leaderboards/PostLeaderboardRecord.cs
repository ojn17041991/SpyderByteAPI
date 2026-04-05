namespace SpyderByteServices.Models.Leaderboards
{
    public record PostLeaderboardRecord
    {
        public Guid LeaderboardId { get; init; }

        public string Player { get; init; } = string.Empty;

        public long Score { get; init; }

        public DateTime? Timestamp { get; init; }
    }
}
