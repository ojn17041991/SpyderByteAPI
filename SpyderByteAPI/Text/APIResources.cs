using SpyderByteAPI.Text.Abstract;

namespace SpyderByteResources.Resources
{
    public class APIResources : IStringLookup<string>
    {
        private IDictionary<string, string> resources = new Dictionary<string, string>()
        {
            { "Title", "SpyderByte API" },
            { "Description", "A public, authenticated API used to manage games and related resources for SpyderByte." }
        };

        public string GetResource(string key)
        {
            return resources?[key] ?? string.Empty;
        }
    }
}
