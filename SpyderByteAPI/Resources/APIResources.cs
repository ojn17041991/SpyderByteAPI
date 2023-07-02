using SpyderByteAPI.Resources.Abstract;

namespace SpyderByteAPI.Resources
{
    public class APIResources : IStringLookup<string>
    {
        private IDictionary<string, string> resources = new Dictionary<string, string>()
        {
            { "Title", "SpyderByte API" },
            { "Description", "A private API for games and related resources developed by SpyderByteStudios." }
        };

        public string GetResource(string key)
        {
            return resources?[key] ?? string.Empty;
        }
    }
}
