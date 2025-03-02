﻿using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;

namespace SpyderByteResources.Resources
{
    public class HttpErrorMessageLookup : IStringLookup<ModelResult>
    {
        private IDictionary<ModelResult, string> resources = new Dictionary<ModelResult, string>()
        {
            { ModelResult.NotImplemented, "This endpoint is no longer supported." },
            { ModelResult.NotFound, "Failed to locate resource." },
            { ModelResult.AlreadyExists, "This resource already exists." },
            { ModelResult.RequestDataIncomplete, "The request data is incomplete." },
            { ModelResult.RequestInvalid, "The request is invalid." },
            { ModelResult.RelationshipViolation, "The request would break a relationship." }
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
