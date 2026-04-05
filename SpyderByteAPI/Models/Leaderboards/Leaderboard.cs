namespace SpyderByteAPI.Models.Leaderboards
{
    public record Leaderboard
    {
        public Guid Id { get; init; }

        public Guid GameId { get; init; }

        public ICollection<LeaderboardRecord> LeaderboardRecords { get; init; } = new List<LeaderboardRecord>();
    }
}
