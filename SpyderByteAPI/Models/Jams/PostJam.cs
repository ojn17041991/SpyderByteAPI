namespace SpyderByteAPI.Models.Jams
{
    public class PostJam
    {
        public string Name { get; set; } = string.Empty;

        public string ItchUrl { get; set; } = string.Empty;

        public IFormFile Image { get; set; }
    }
}
