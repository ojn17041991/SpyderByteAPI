using SpyderByteDataAccess.Models.Leaderboards;
using SpyderByteDataAccess.Models.Users;
using SpyderByteResources.Enums;
using System.Text.Json.Serialization;

namespace SpyderByteDataAccess.Models.Games
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

        [JsonIgnore]
        public LeaderboardGame? LeaderboardGame { get; set; }

        [JsonIgnore]
        public UserGame? UserGame { get; set; }
    }
}
