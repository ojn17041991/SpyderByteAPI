using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboard
{
    public class PostLeaderboardRecord
    {
        [Required]
        public Guid LeaderboardId { get; set; }

        [Required]
        public string Player { get; set; } = string.Empty;

        [Required]
        public long Score { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
