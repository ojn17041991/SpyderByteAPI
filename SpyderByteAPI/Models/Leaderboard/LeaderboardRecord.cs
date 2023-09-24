namespace SpyderByteAPI.Models.Leaderboard
{
    public class LeaderboardRecord
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }

        public string Player { get; set; } = string.Empty;

        public long Score { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
