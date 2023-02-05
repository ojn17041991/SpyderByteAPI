using SpyderByteAPI.Enums;

namespace SpyderByteAPI.Models.Abstract
{
    public interface IWeb
    {
        IList<Browser>? SupportedBrowsers { get; set; }
    }
}
