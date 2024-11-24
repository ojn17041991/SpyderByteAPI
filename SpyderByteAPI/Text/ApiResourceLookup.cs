using SpyderByteAPI.Text.Abstract;

namespace SpyderByteResources.Resources
{
    public class ApiResourceLookup : IStringLookup<string>
    {
        private IDictionary<string, string> resources = new Dictionary<string, string>()
        {
            { "title", "SpyderByte API" },
            { "description", "A public, authenticated API used to manage games and related resources for SpyderByte." }
        };

        public string GetResource(string key)
        {
            if (resources.ContainsKey(key))
            {
                return resources[key];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
