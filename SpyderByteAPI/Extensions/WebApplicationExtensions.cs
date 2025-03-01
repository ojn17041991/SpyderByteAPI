using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.Middleware; // Required for release.
using SpyderByteDataAccess.Contexts;
using SpyderByteResources.Resources;

namespace SpyderByteResources.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void AddSwaggerConfiguration(this WebApplication webApplication, ConfigurationManager configuration)
        {
            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI(options =>
                {
                    var apiResources = new ApiResourceLookup();
                    string apiName = apiResources.GetResource("title");

                    var apiVersionDescriptionProvider = webApplication.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var versionDescriptor in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{versionDescriptor.GroupName}/swagger.json",
                            $"{apiName} v{versionDescriptor.ApiVersion.MajorVersion}.{versionDescriptor.ApiVersion.MinorVersion}"
                        );
                    }
                });
            }
        }

        public static void AddProjectMiddleware(this WebApplication webApplication)
        {
            #if !DEBUG
                webApplication.UseMiddleware<RequestBodyLogger>();
            #endif
        }

        public static void RunProjectMigrations(this WebApplication webApplication)
        {
            using (var serviceScope = webApplication.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                if (serviceScope != null)
                {
                    var dbContext = serviceScope.ServiceProvider?.GetService<ApplicationDbContext>();
                    if (dbContext != null)
                    {
                        dbContext.Database?.Migrate();
                    }
                }
            }
        }
    }
}
