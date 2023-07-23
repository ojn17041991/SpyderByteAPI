using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PostGame
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string HtmlUrl { get; set; } = string.Empty;

        [Required]
        public IFormFile? Image { get; set; }

        [Required]
        public DateTime? PublishDate { get; set; }
    }
}
