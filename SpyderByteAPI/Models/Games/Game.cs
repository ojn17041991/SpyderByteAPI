using SpyderByteAPI.Models.Leaderboard;
using SpyderByteAPI.Models.Users;
using System.Text.Json.Serialization;

namespace SpyderByteAPI.Models.Games
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

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
