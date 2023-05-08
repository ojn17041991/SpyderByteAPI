using SpyderByteAPI.Enums;
using SpyderByteAPI.Resources.Abstract;

namespace SpyderByteAPI.Resources
{
    public class ModelResources : IStringLookup<ModelResult>
    {
        private IDictionary<ModelResult, string> languageResources = new Dictionary<ModelResult, string>()
        {
            { ModelResult.OK, "The request was served successfully." },
            { ModelResult.Created, "The resource was created successfully." },
            { ModelResult.Error, "A server error occurred while processing the request." },
            { ModelResult.NotFound, "Failed to locate resource." },
            { ModelResult.AlreadyExists, "This resource already exists." }
        };

        public string GetResource(ModelResult modelResult)
        {
            return languageResources?[modelResult] ?? string.Empty;
        }
    }
}
