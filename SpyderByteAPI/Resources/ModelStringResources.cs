using SpyderByteAPI.Enums;
using SpyderByteAPI.Resources.Abstract;

namespace SpyderByteAPI.Resources
{
    public class ModelStringResources : IStringResources<ModelResult>
    {
        private IDictionary<ModelResult, string> languageResources = new Dictionary<ModelResult, string>()
        {
            { ModelResult.OK, "The request was executed successfully." },
            { ModelResult.Error, "A server error occurred while processing the request." },
            { ModelResult.NotFound, "Could not locate the resource specified." },
            { ModelResult.IDGivenForIdentityField, "This type of request cannot include an ID value." },
            { ModelResult.IDMismatchInPut, "The ID in the header does not match the ID in the body." }
        };

        public string GetResource(ModelResult modelResult)
        {
            return languageResources?[modelResult] ?? string.Empty;
        }
    }
}
