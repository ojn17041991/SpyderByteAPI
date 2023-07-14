using AspNetCoreRateLimit;
using Azure.Identity; // Required for Release.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.ApplicationInsights; // Required for Release.
using Microsoft.OpenApi.Models;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Resources;
using SpyderByteAPI.Resources.Abstract;
using SpyderByteAPI.Services.Imgur;
using SpyderByteAPI.Services.Imgur.Abstract;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressMapClientErrors = true);
builder.Services.AddEndpointsApiExplorer();

APIResources apiResources = new APIResources();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = apiResources.GetResource("Title"),
        Description = apiResources.GetResource("Description"),
        Version = "1.0.0.0"
    });
});

builder.Services.AddScoped<IGamesAccessor, GamesAccessor>();
builder.Services.AddScoped<IJamsAccessor, JamsAccessor>();
builder.Services.AddScoped<ILeaderboardAccessor, LeaderboardAccessor>();

builder.Services.AddScoped<IImgurService, ImgurService>();

builder.Services.AddScoped<IStringLookup<ModelResult>, ModelResources>();

string? connectionString = builder.Configuration.GetConnectionString("SbApi");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        connectionString
    ),
    ServiceLifetime.Scoped
);

builder.Services.AddHttpClient();
builder.Services.AddCors(p => p.AddPolicy("SpyderByteAPI", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

#if !DEBUG
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddLogging(logBuilder => logBuilder.AddApplicationInsights());

builder.Configuration.AddAzureKeyVault(
    new Uri("https://spyderbyteglobalkeyvault.vault.azure.net/"),
    new DefaultAzureCredential()
);
#endif

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.HttpStatusCode = 429;
    options.GeneralRules = new List<RateLimitRule>
    {
        // Rule is per IP per endpoint.
        new RateLimitRule
        {
            Endpoint = "GET:*",
            Period = "60s",
            Limit = 20
        },
        new RateLimitRule
        {
            Endpoint = "POST:*",
            Period = "60s",
            Limit = 50 // Need this to be able to run the auto-populate program.
        },
        new RateLimitRule
        {
            Endpoint = "PATCH:*",
            Period = "60s",
            Limit = 5
        },
        new RateLimitRule
        {
            Endpoint = "DELETE:*",
            Period = "60s",
            Limit = 5
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("SpyderByteAPI");
app.UseIpRateLimiting();

using (var serviceScope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
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

app.Run();