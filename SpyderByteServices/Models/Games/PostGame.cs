using Microsoft.AspNetCore.Http;
using SpyderByteResources.Enums;

namespace SpyderByteServices.Models.Games
{
    public class PostGame
    {
        public string Name { get; set; } = string.Empty;

        public GameType Type { get; set; }

        public string Url { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    }
}
