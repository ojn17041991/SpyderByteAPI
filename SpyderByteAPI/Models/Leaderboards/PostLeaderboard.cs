using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Leaderboards
{
    public class PostLeaderboard
    {
        [Required]
        public Guid GameId { get; set; }
    }
}
