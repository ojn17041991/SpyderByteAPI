using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SpyderByteResources.Resources;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpyderByteAPI.Middleware
{
    public class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;
        private readonly ApiResourceLookup apiResourceLookup;

        public SwaggerConfigureOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            this.apiVersionDescriptionProvider = apiVersionDescriptionProvider;
            apiResourceLookup = new ApiResourceLookup();
        }

        public void Configure(SwaggerGenOptions options)
        {
            string apiName = apiResourceLookup.GetResource("title");
            string apiDescription = apiResourceLookup.GetResource("description");

            foreach (var versionDescriptor in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(versionDescriptor.GroupName, new OpenApiInfo()
                {
                    Title = apiName,
                    Description = apiDescription,
                    Version = $"{versionDescriptor.ApiVersion.MajorVersion}.{versionDescriptor.ApiVersion.MinorVersion}"
                });
            }
        }
    }
}
