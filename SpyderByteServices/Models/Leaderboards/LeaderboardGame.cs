using SpyderByteServices.Models.Games;

namespace SpyderByteServices.Models.Leaderboards
{
    public class LeaderboardGame
    {
        public int Id { get; set; }

        public Guid GameId { get; set; }

        public Game Game { get; set; } = new Game();

        public Guid LeaderboardId { get; set; }

        public Leaderboard Leaderboard { get; set; } = new Leaderboard();
    }
}
