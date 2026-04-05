using Microsoft.AspNetCore.Http;
using SpyderByteResources.Enums;

namespace SpyderByteServices.Models.Games
{
    public record PostGame
    {
        public string Name { get; init; } = string.Empty;

        public GameType Type { get; init; }

        public string Url { get; init; } = string.Empty;

        public IFormFile? Image { get; init; }

        public DateTime PublishDate { get; init; } = DateTime.UtcNow;
    }
}
