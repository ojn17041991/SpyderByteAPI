namespace SpyderByteAPI.Models.Leaderboards
{
    public record Leaderboard
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }

        public ICollection<LeaderboardRecord> LeaderboardRecords { get; set; } = new List<LeaderboardRecord>();
    }
}
