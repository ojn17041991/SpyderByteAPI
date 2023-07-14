namespace SpyderByteAPI.Models.Games
{
    public class Game
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.MinValue;
    }
}
