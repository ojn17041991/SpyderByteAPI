using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards
{
    public class PostLeaderboardRecord
    {
        [Required]
        public Guid LeaderboardId { get; set; }

        [Required]
        [RegularExpression(@"[^<>\\\/\r\n]{1,50}", ErrorMessage = "Player does not meet validation requirements.")]
        public string Player { get; set; } = string.Empty;

        [Required]
        public long Score { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
