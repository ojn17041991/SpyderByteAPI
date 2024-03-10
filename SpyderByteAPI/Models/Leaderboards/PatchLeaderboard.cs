using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards
{
    public class PatchLeaderboard
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid GameId { get; set; }
    }
}
