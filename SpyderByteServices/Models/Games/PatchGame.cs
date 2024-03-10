using Microsoft.AspNetCore.Http;
using SpyderByteResources.Enums;

namespace SpyderByteServices.Models.Games
{
    public class PatchGame
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public GameType? Type { get; set; }

        public string? HtmlUrl { get; set; }

        public IFormFile? Image { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
