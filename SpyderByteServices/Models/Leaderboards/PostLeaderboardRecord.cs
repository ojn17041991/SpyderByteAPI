using System.ComponentModel.DataAnnotations;

namespace SpyderByteServices.Models.Leaderboards
{
    public class PostLeaderboardRecord
    {
        public Guid LeaderboardId { get; set; }

        public string Player { get; set; } = string.Empty;

        public long Score { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
