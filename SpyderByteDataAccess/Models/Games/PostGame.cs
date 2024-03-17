using SpyderByteResources.Enums;

namespace SpyderByteDataAccess.Models.Games
{
    public class PostGame
    {
        public string Name { get; set; } = string.Empty;

        public GameType Type { get; set; }

        public string Url { get; set; } = string.Empty;

        public string ImgurUrl { get; set; } = string.Empty;

        public string ImgurImageId { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    }
}
