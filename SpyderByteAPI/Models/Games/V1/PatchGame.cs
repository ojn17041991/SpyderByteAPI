using SpyderByteAPI.Attributes;
using SpyderByteResources.Enums;
using System.ComponentModel.DataAnnotations;

namespace SpyderByteAPI.Models.Games.V1
{
    public record PatchGame
    {
        [Required]
        public Guid Id { get; init; }

        [RegularExpression(@"[^<>\\\/\r\n]{1,50}", ErrorMessage = "Name does not meet validation requirements.")]
        public string? Name { get; init; }

        public GameType? Type { get; init; }

        public string? Url { get; init; }

        [FileUploadValidation(ExistingResource = true, ErrorMessage = "Image file can only be of type png.")]
        public IFormFile? Image { get; init; }

        public DateTime? PublishDate { get; init; }
    }
}
