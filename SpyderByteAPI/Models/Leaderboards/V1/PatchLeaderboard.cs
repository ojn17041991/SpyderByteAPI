using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards.V1
{
    public record PatchLeaderboard
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid GameId { get; set; }
    }
}
