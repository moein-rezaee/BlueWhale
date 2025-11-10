using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using MediatR;
using Serilog;
using dotenv.net;
using BlueWhale.Registry.Infrastructure.Persistence;
using BlueWhale.Registry.Application.Options;
using BlueWhale.Registry.Application;
using BlueWhale.Registry.Infrastructure.ExternalServices;
using BlueWhale.Registry.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BlueWhale Docker Registry API",
        Version = "v1.0",
        Description = "Complete Docker Registry Management Panel API - Control Docker Registry repositories, images, tags and view statistics"
    });
});

// Configure Options
var registryOptions = builder.Configuration.GetSection("Registry").Get<RegistryOptions>() ?? new RegistryOptions();
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
var cacheOptions = builder.Configuration.GetSection("Cache").Get<CacheOptions>() ?? new CacheOptions();

builder.Services.Configure<RegistryOptions>(builder.Configuration.GetSection("Registry"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));

// Configure Database
var dbPath = builder.Configuration["Database:SqlitePath"] ?? "data/registry.db";
Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? "data");

builder.Services.AddDbContext<RegistryDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Register Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configure HttpClient for Docker Registry
builder.Services.AddHttpClient<IDockerRegistryService, DockerRegistryService>(client =>
{
    client.BaseAddress = new Uri(registryOptions.RegistryUrl);
    client.Timeout = TimeSpan.FromSeconds(registryOptions.TimeoutSeconds);
});

// Configure MediatR and Validation
builder.Services.AddMediatR(config => 
    config.RegisterServicesFromAssemblyContaining(typeof(AssemblyMarker)));
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "bluewhale-docker-registry-secret-key-2025";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BlueWhale";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BlueWhale-UI";

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Logger.Warning($"JWT authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Configure Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RegistryDbContext>();
    await db.Database.EnsureCreatedAsync();
    
    // Seed admin user if needed
    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var adminExists = await unitOfWork.Users.UsernameExistsAsync("admin");
    if (!adminExists)
    {
        var adminUser = new BlueWhale.Registry.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@bluewhale.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = BlueWhale.Registry.Domain.Entities.UserRole.Admin,
            IsActive = true
        };
        await unitOfWork.Users.AddAsync(adminUser);
        await unitOfWork.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BlueWhale Registry API v1");
        options.RoutePrefix = "swagger";
    });
}

// Determine URL scheme
var scheme = app.Environment.IsDevelopment() ? "http" : "https";
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(';')[0];
    scheme = urls.StartsWith("https") ? "https" : "http";
}

var host = app.Configuration["ASPNETCORE_HOST"] ?? "localhost";
var port = app.Configuration["ASPNETCORE_PORT"] ?? "5260";
var baseUrl = $"{scheme}://{host}:{port}";

// Root status endpoint
app.MapGet("/", () => Results.Text(
    $"🐳 BlueWhale Docker Registry API\n" +
    $"{"━".PadRight(50, '━')}\n" +
    $"Environment: {app.Environment.EnvironmentName}\n" +
    $"Base URL: {baseUrl}\n" +
    $"API Version: v1\n" +
    $"API Prefix: /v1/api\n" +
    $"\n" +
    $"Endpoints:\n" +
    $"  • Health: {baseUrl}/v1/api/health\n" +
    $"  • Repositories: {baseUrl}/v1/api/repositories\n" +
    $"  • Tags: {baseUrl}/v1/api/tags/{{repository}}\n" +
    $"  • Statistics: {baseUrl}/v1/api/statistics/summary\n" +
    $"  • Swagger UI: {baseUrl}/swagger\n" +
    $"\n" +
    $"Status: ✅ Running\n" +
    $"Time: {DateTime.UtcNow:O}",
    "text/plain"));

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
