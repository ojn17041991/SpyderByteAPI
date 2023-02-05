using SpyderByteAPI.Enums;
using SpyderByteAPI.Models.Abstract;

namespace SpyderByteAPI.Models
{
    public class WebGame : Game, IWeb
    {
        public IList<Browser>? SupportedBrowsers { get; set; } = new List<Browser>();
    }
}
