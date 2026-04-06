using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards
{
    public record PostLeaderboard
    {
        [Required]
        public Guid GameId { get; init; }
    }
}
