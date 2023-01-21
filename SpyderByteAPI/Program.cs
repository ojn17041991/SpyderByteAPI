using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;
using SpyderByteAPI.Resources;
using SpyderByteAPI.Resources.Abstract;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
    s.EnableAnnotations()
);

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