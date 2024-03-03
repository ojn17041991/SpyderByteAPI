using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboard
{
    public class PostLeaderboard
    {
        [Required]
        public Guid GameId { get; set; }
    }
}
