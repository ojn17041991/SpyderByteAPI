using SpyderByteResources.Enums;

namespace SpyderByteDataAccess.Models.Games
{
    public class PatchGame
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public GameType? Type { get; set; }

        public string? Url { get; set; }

        public string? ImgurUrl { get; set; }

        public string? ImgurImageId { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
