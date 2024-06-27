using SpyderByteResources.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public class PostGame
    {
        [Required]
        [RegularExpression(@"[^<>\\/]{1,50}", ErrorMessage = "Name does not meet validation requirements.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public GameType Type { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        [FileExtensions(Extensions = "png")]
        public IFormFile? Image { get; set; }

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    }
}
