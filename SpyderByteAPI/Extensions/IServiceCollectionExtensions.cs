﻿using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SpyderByteAPI.Middleware; // Required for release.
using Azure.Identity; // Required for release.
using SpyderByteDataAccess.Accessors.Games.Abstract;
using SpyderByteDataAccess.Accessors.Games;
using SpyderByteDataAccess.Accessors.Leaderboards.Abstract;
using SpyderByteDataAccess.Accessors.Leaderboards;
using SpyderByteDataAccess.Accessors.Users;
using SpyderByteDataAccess.Accessors.Users.Abstract;
using SpyderByteServices.Services.Authentication.Abstract;
using SpyderByteServices.Services.Data.Abstract;
using SpyderByteServices.Services.Data;
using SpyderByteServices.Services.Authentication;
using SpyderByteServices.Services.Users.Abstract;
using SpyderByteServices.Services.Users;
using SpyderByteServices.Services.Storage.Abstract;
using SpyderByteServices.Services.Storage;
using SpyderByteServices.Services.Imgur;
using SpyderByteServices.Services.Imgur.Abstract;
using SpyderByteAPI.Text.Abstract;
using SpyderByteResources.Enums;
using SpyderByteResources.Resources;
using SpyderByteDataAccess.Contexts;
using SpyderByteResources.Helpers.Authorization;
using SpyderByteServices.Services.Games.Abstract;
using SpyderByteServices.Services.Games;
using SpyderByteServices.Services.Leaderboards.Abstract;
using SpyderByteServices.Services.Leaderboards;
using SpyderByteServices.Services.Password;
using SpyderByteServices.Services.Password.Abstract;
using Microsoft.FeatureManagement;
using Asp.Versioning;
using Microsoft.Extensions.Azure;
using SpyderByteServices.Services.Encoding;
using SpyderByteServices.Services.Encoding.Abstract;
using SpyderByteResources.Flags;
using SpyderByteResources.Paging.Factories;
using SpyderByteDataAccess.Paging.Factories.Abstract;
using SpyderByteDataAccess.Transactions.Factories.Abstract;
using SpyderByteDataAccess.Transactions.Factories;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SpyderByteResources.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped<ITransactionFactory, TransactionFactory>();
            services.AddScoped<IGamesAccessor, GamesAccessor>();
            services.AddScoped<ILeaderboardsAccessor, LeaderboardsAccessor>();
            services.AddScoped<IUsersAccessor, UsersAccessor>();
            services.AddScoped<IPagedListFactory, PagedListFactory>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<SpyderByteServices.Services.Authorization.Abstract.IAuthorizationService, SpyderByteServices.Services.Authorization.AuthorizationService>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IGamesService, GamesService>();
            services.AddScoped<ILeaderboardsService, LeaderboardsService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IImgurService, ImgurService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEncodingService, EncodingService>();

            services.AddScoped<IStringLookup<ModelResult>, HttpErrorMessageLookup>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        }

        public static void AddProjectDatabase(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("SbApi");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    connectionString
                ),
                ServiceLifetime.Scoped
            );
        }

        public static void AddProjectCors(this IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy("SpyderByteAPIOrigins", builder =>
                {
                    builder
                        .WithOrigins("https://spyderbytestudios.itch.io",
                                     "https://www.spyderbyte.co.uk")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                })
            );
        }

        public static void AddProjectAzureServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            #if !DEBUG
                services.AddApplicationInsightsTelemetry();
                services.AddLogging(logBuilder => logBuilder.AddApplicationInsights());

                services.AddTransient<RequestBodyLogger>();

                configuration.AddAzureKeyVault(
                    new Uri("https://spyderbyteglobalkeyvault.vault.azure.net/"),
                    new DefaultAzureCredential()
                );
            #endif

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(configuration.GetConnectionString("Storage")).WithName(configuration["Storage:ClientName"]);
            });
        }

        public static void AddProjectRateLimiting(this IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(options =>
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
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();
        }

        public static void AddProjectAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidAudience = "SpyderByteWebApplication",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:EncodingKey"] ?? string.Empty)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void AddProjectAuthorization(this IServiceCollection services)
        {
            services.Configure<AuthorizationOptions>(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                    {
                        var token = (context.Resource as HttpContext).GetToken();
                        if (token.IsNullOrEmpty()) return false;

                        var isTokenBlacklisted = TokenBlacklister.IsTokenBlacklisted(token);
                        return !isTokenBlacklisted;
                    })
                    .Build();

                options.AddPolicy(PolicyType.ReadUsers,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.ReadUsers.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.WriteUsers,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.WriteUsers.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.WriteGames,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.WriteGames.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.ReadLeaderboards,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.ReadLeaderboards.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.WriteLeaderboards,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.WriteLeaderboards.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.WriteLeaderboardRecords,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.WriteLeaderboardRecords.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.DeleteLeaderboards,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.DeleteLeaderboards.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.DeleteLeaderboardRecords,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.DeleteLeaderboardRecords.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.DataBackup,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.DataBackup.ToDescription())
                    .Build()
                );

                options.AddPolicy(PolicyType.DataCleanup,
                    new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimType.DataCleanup.ToDescription())
                    .Build()
                );
            });
        }

        public static void AddProjectVersioning(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        public static void AddProjectMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(SpyderByteAPI.Mappers.MapperProfile).Assembly);
            services.AddAutoMapper(typeof(SpyderByteServices.Mappers.MapperProfile).Assembly);
            services.AddAutoMapper(typeof(SpyderByteResources.Mappers.MapperProfile).Assembly);
        }

        public static void AddProjectFeatureFlags(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddFeatureManagement(configuration.GetSection("FeatureFlags"));
        }
    }
}
