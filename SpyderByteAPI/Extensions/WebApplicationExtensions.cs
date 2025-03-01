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
                webApplication.UseSwaggerUI();// x =>
                //{
                    //var apiResources = new ApiResourceLookup();
                    //string apiName = apiResources.GetResource("title");

                    //IDictionary<string, string>[] versions = configuration.GetSection("Versioning:Supported").Get<Dictionary<string, string>[]>()!;

                    //foreach (IDictionary<string, string> version in versions)
                    //{
                    //    string major = version["Major"];
                    //    string minor = version["Minor"];
                    //    string patch = version["Patch"];

                    //    x.SwaggerEndpoint($"/swagger/v{major}.{minor}/swagger.json", $"{apiName} v{major}.{minor}");
                    //}
                //});
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
