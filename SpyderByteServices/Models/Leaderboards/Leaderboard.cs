namespace SpyderByteServices.Models.Leaderboards
{
    public class Leaderboard
    {
        public Guid Id { get; set; }

        public LeaderboardGame LeaderboardGame { get; set; } = new LeaderboardGame();

        public ICollection<LeaderboardRecord> LeaderboardRecords { get; set; } = new List<LeaderboardRecord>();
    }
}
