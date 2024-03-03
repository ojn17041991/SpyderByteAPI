using SpyderByteAPI.Models.Games;

namespace SpyderByteAPI.Models.Users
{
    public class UserGame
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = new User();

        public Guid GameId { get; set; }

        public Game Game { get; set; } = new Game();
    }
}
