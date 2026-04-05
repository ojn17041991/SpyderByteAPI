namespace SpyderByteServices.Models.Leaderboards
{
    public record Leaderboard
    {
        public Guid Id { get; init; }

        public LeaderboardGame LeaderboardGame { get; init; } = null!;

        public ICollection<LeaderboardRecord> LeaderboardRecords { get; init; } = new List<LeaderboardRecord>();
    }
}
