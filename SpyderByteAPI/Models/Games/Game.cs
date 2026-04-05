using SpyderByteResources.Enums;

namespace SpyderByteAPI.Models.Games
{
    public record Game
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public GameType Type { get; init; }

        public string Url { get; init; } = string.Empty;

        public string ImgurUrl { get; init; } = string.Empty;

        public string ImgurImageId { get; init; } = string.Empty;

        public DateTime PublishDate { get; init; } = DateTime.MinValue;

        public Guid? LeaderboardId { get; init; }

        public Guid? UserId { get; init; }
    }
}
