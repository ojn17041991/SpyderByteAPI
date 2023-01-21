using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;
using SpyderByteAPI.Resources;
using SpyderByteAPI.Resources.Abstract;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressMapClientErrors = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SpyderByte API",
        Description = "An repository API for game information developed by SpyderByteStudios.",
        Version = "1.0.3.1"
    });
});

builder.Services.AddTransient<IDataAccessor<Game>, GamesAccessor>();
builder.Services.AddTransient<IStringResources<ModelResult>, ModelStringResources>();

// Replace the SQLite data directory with a relative path.
string? connectionString = builder.Configuration.GetConnectionString("Games");
string? assemblyLocation = Assembly.GetExecutingAssembly().Location;
string? dataDirectory = Path.GetDirectoryName(assemblyLocation);
connectionString = connectionString?.Replace("|DataDirectory|", dataDirectory) ?? string.Empty;

// Send the updated connection string to the DB Context.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        connectionString
    )
);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();