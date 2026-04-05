using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards
{
    public record PostLeaderboardRecord
    {
        [Required]
        public Guid LeaderboardId { get; init; }

        [Required]
        [RegularExpression(@"[^<>\\\/\r\n]{1,50}", ErrorMessage = "Player does not meet validation requirements.")]
        public string Player { get; init; } = string.Empty;

        [Required]
        public long Score { get; init; }

        public DateTime? Timestamp { get; init; }
    }
}
