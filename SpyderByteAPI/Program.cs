using AspNetCoreRateLimit;
using Azure.Identity; // Required for Release.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract.Accessors;
using SpyderByteAPI.DataAccess.Accessors;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Helpers;
using SpyderByteAPI.Middleware; // Required for Release.
using SpyderByteAPI.Resources;
using SpyderByteAPI.Resources.Abstract;
using SpyderByteAPI.Services.Auth;
using SpyderByteAPI.Services.Auth.Abstract;
using SpyderByteAPI.Services.Imgur;
using SpyderByteAPI.Services.Imgur.Abstract;
using System.Text;

// OJN: This whole file needs to be modularized.

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

builder.Services.AddSingleton<IImgurService, ImgurService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IStringLookup<ModelResult>, ModelResources>();

string? connectionString = builder.Configuration.GetConnectionString("SbApi");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        connectionString
    ),
    ServiceLifetime.Scoped
);

builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
    options.AddPolicy("SpyderByteAPIOrigins", builder =>
    {
        builder
            .WithOrigins("https://spyderbytestudios.itch.io/*",
                         "https://www.spyderbyte.co.uk")
            .AllowAnyMethod()
            .AllowAnyHeader();
    })
);

#if !DEBUG
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddLogging(logBuilder => logBuilder.AddApplicationInsights());

builder.Services.AddTransient<RequestBodyToInsightMiddleware>();

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
            Limit = 10
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:EncodingKey"] ?? string.Empty)),
        ClockSkew = TimeSpan.Zero
    };
});

// OJN: Can you get this to return 401, not 403?
builder.Services.Configure<AuthorizationOptions>(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireAssertion(context =>
    {
        var token = AuthenticationHelper.GetTokenFromHttpContext(context.Resource as HttpContext);
        if (token.IsNullOrEmpty()) return false;

        var isTokenBlacklisted = AuthenticationHelper.IsTokenBlacklisted(token);
        return !isTokenBlacklisted;
    })
    .Build();
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("SpyderByteAPI");
app.UseIpRateLimiting();

#if !DEBUG
    app.UseMiddleware<RequestBodyToInsightMiddleware>();
#endif

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