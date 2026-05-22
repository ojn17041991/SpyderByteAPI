using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;

namespace SpyderByteResources.Resources
{
    public class HttpErrorMessageLookup : IStringLookup<ModelResult>
    {
        private IDictionary<ModelResult, string> resources = new Dictionary<ModelResult, string>()
        {
            { ModelResult.AlreadyExists, "This resource already exists." },
            { ModelResult.Created, "The resource was created." },
            { ModelResult.Error, "An unexpected error occurred." },
            { ModelResult.Forbidden, "Unauthorized access." },
            { ModelResult.ImageDeletionFailed, "The image was not deleted." },
            { ModelResult.NotFound, "Failed to locate resource." },
            { ModelResult.NotImplemented, "This endpoint is no longer supported." },
            { ModelResult.OK, "The request was successful." },
            { ModelResult.RelationshipViolation, "The request would break a relationship." },
            { ModelResult.RequestDataIncomplete, "The request data is incomplete." },
            { ModelResult.RequestInvalid, "The request is invalid." },
            { ModelResult.Unauthorized, "Unauthenticated user." },
        };

        public string GetResource(ModelResult modelResult)
        {
            if (resources.ContainsKey(modelResult))
            {
                return resources[modelResult];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
