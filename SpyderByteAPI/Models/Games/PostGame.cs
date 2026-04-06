using SpyderByteAPI.Attributes;
using SpyderByteResources.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games
{
    public record PostGame
    {
        [Required]
        [RegularExpression(@"[^<>\\\/\r\n]{1,50}", ErrorMessage = "Name does not meet validation requirements.")]
        public string Name { get; init; } = string.Empty;

        [Required]
        public GameType Type { get; init; }

        [Required]
        public string Url { get; init; } = string.Empty;

        [Required]
        [FileUploadValidation(ExistingResource = false, ErrorMessage = "Image file can only be of type png.")]
        public IFormFile? Image { get; init; }

        [Required]
        public DateTime PublishDate { get; init; } = DateTime.UtcNow;
    }
}
