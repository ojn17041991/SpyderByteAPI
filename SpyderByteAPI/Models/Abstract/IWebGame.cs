using SpyderByteAPI.Enums;

namespace SpyderByteAPI.Models.Abstract
{
    public interface IWebGame : IGame
    {
        IList<Browser> SupportedBrowsers { get; set; }
    }
}
