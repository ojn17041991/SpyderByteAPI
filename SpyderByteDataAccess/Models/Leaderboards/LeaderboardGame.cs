using SpyderByteDataAccess.Models.Games;

namespace SpyderByteDataAccess.Models.Leaderboards
{
    public class LeaderboardGame
    {
        public int Id { get; set; }

        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;

        public Guid LeaderboardId { get; set; }

        public Leaderboard Leaderboard { get; set; } = null!;
    }
}
