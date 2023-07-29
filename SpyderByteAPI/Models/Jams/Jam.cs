namespace SpyderByteAPI.Models.Jams
{
    public class Jam
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ItchUrl { get; set; } = string.Empty;

        public string ImgurUrl { get; set; } = string.Empty;

        public string ImgurImageId { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; } = DateTime.MinValue;
    }
}
