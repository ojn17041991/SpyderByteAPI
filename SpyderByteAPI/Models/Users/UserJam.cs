using SpyderByteAPI.Models.Jams;

namespace SpyderByteAPI.Models.Users
{
    public class UserJam
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public Guid JamId { get; set; }

        public Jam Jam { get; set; }
    }
}
