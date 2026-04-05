using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards.V1
{
    public record PatchLeaderboard
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public Guid GameId { get; init; }
    }
}
