using SpyderByteAPI.Enums;
using System.Text.Json.Serialization;

namespace SpyderByteAPI.Models.Users
{
    public class User
    {
        public string Id { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public UserType UserType { get; set; } = UserType.Restricted;

        [JsonIgnore]
        public UserJam? UserJam { get; set; }
    }
}
