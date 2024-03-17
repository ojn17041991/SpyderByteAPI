using SpyderByteAPI.Models.Leaderboards;
using SpyderByteAPI.Models.Users;
using SpyderByteResources.Enums;
using System.Text.Json.Serialization;

namespace SpyderByteAPI.Models.Games
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public GameType Type { get; set; }

        public string Url { get; set; } = string.Empty;

        public string ImgurUrl { get; set; } = string.Empty;

        public string ImgurImageId { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.MinValue;

        public Guid? LeaderboardId { get; set; }

        public Guid? UserId { get; set; }
    }
}
