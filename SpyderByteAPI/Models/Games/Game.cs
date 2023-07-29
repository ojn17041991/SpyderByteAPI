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
    }
}
