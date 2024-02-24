using AspNetCoreRateLimit;
using SpyderByteAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressMapClientErrors = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProjectVersioning();
builder.Services.AddProjectDependencies();
builder.Services.AddProjectDatabase(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddProjectCors();
builder.Services.AddProjectAzureServices(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddProjectRateLimiting();
builder.Services.AddProjectAuthentication(builder.Configuration);
builder.Services.AddProjectAuthorization();

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
app.AddProjectMiddleware();
app.RunProjectMigrations();
app.Run();