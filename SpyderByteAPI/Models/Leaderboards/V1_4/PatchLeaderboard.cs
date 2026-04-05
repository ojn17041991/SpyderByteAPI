using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards.V1_4
{
    public record PatchLeaderboard
    {
        [Required]
        public Guid GameId { get; init; }
    }
}
