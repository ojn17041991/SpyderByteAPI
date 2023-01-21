using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Abstract;

namespace SpyderByteAPI.Models
{
    public class WebGame : IWebGame
    {
        // Web game needs all these properties, but we only really need to save these 2:
        // Then you can join Game with WebGame to get all the information.
        // WebGame would contain ONLY the web game information.
        public IList<Browser> SupportedBrowsers { get; set; } = new List<Browser>();

        public int? Id { get; set; }



        public string? Name { get; set; } = string.Empty;
        public DateTime? PublishDate { get; set; } = DateTime.MinValue;
    }
}
