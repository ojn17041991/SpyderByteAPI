namespace SpyderByteServices.Models.Leaderboards
{
    public class LeaderboardRecord
    {
        public Guid Id { get; set; }

        public Guid LeaderboardId { get; set; }

        public Leaderboard Leaderboard { get; set; } = new Leaderboard();

        public string Player { get; set; } = string.Empty;

        public long Score { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
