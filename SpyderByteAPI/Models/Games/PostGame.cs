using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PostGame
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    }
}
