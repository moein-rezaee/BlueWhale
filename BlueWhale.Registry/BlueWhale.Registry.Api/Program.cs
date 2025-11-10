using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using Serilog;
using BlueWhale.Registry.Infrastructure.Persistence;
using BlueWhale.Registry.Application.Options;
using BlueWhale.Registry.Application;
using BlueWhale.Registry.Infrastructure.ExternalServices;
using BlueWhale.Registry.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BlueWhale Registry API",
        Version = "v1",
        Description = "Docker Registry Management Panel API"
    });
});

var registryOptions = builder.Configuration.GetSection("Registry").Get<RegistryOptions>() ?? new RegistryOptions();
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

builder.Services.Configure<RegistryOptions>(builder.Configuration.GetSection("Registry"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));

builder.Services.AddDbContext<RegistryDbContext>(options =>
    options.UseSqlite("Data Source=registry.db"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpClient<IDockerRegistryService, DockerRegistryService>(client =>
{
    client.BaseAddress = new Uri(registryOptions.RegistryUrl);
    client.Timeout = TimeSpan.FromSeconds(registryOptions.TimeoutSeconds);
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining(typeof(AssemblyMarker)));
builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RegistryDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BlueWhale Registry API v1");
    });
}

var scheme = app.Environment.IsDevelopment() ? "http" : "https";
var host = app.Configuration["ASPNETCORE_HOST"] ?? "localhost";
var port = app.Configuration["ASPNETCORE_PORT"] ?? "5280";
var baseUrl = $"{scheme}://{host}:{port}";

app.MapGet("/", () => Results.Text(
    $"BlueWhale Registry API\n" +
    $"Environment: {app.Environment.EnvironmentName}\n" +
    $"Base URL: {baseUrl}\n" +
    $"API Prefix: /v1/api\n" +
    $"Swagger: {baseUrl}/swagger\n" +
    $"Time: {DateTime.UtcNow:O}",
    "text/plain"));

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
