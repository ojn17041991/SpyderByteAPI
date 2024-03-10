using SpyderByteResources.Enums;
using SpyderByteServices.Models.Leaderboards;
using SpyderByteServices.Models.Users;

namespace SpyderByteServices.Models.Games
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public GameType Type { get; set; }

        public string HtmlUrl { get; set; } = string.Empty;

        public string ImgurUrl { get; set; } = string.Empty;

        public string ImgurImageId { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.MinValue;

        public LeaderboardGame? LeaderboardGame { get; set; }

        public UserGame? UserGame { get; set; }
    }
}
