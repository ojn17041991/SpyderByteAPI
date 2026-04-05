using SpyderByteResources.Enums;
using SpyderByteServices.Models.Leaderboards;
using SpyderByteServices.Models.Users;

namespace SpyderByteServices.Models.Games
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

        public LeaderboardGame? LeaderboardGame { get; init; } = null!;

        public UserGame? UserGame { get; init; } = null!;
    }
}
