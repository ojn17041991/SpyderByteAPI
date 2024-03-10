using SpyderByteDataAccess.Models.Games;

namespace SpyderByteDataAccess.Models.Users
{
    public class UserGame
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;
    }
}
