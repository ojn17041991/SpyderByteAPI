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
            { ModelResult.AlreadyExists, "This resource already exists." },
            { ModelResult.RequestDataIncomplete, "The request data is incomplete." },
            { ModelResult.RequestInvalid, "The request is invalid." },
            { ModelResult.RelationshipViolation, "The request would break a relationship." }
        };

        public string GetResource(ModelResult modelResult)
        {
            return languageResources?[modelResult] ?? string.Empty;
        }
    }
}
