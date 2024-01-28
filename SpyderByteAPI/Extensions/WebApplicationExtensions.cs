using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess;

namespace SpyderByteAPI.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void AddProjectMiddleware(this WebApplication webApplication)
        {
            #if !DEBUG
                app.UseMiddleware<RequestBodyToInsightMiddleware>();
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
