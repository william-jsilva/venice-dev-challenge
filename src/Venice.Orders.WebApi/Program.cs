using Venice.Orders.Infrastructure.Services;
using Venice.Orders.WebApi.Configuration;
using Venice.Orders.WebApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add Docker-specific configuration file
if (builder.Environment.EnvironmentName == "Docker")
{
    builder.Configuration.AddJsonFile("appsettings.Docker.json", optional: false, reloadOnChange: true);
}

// Configure MongoDB serialization globally at application startup
MongoDbConfigurationService.ConfigureSerialization();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure application services
builder.Services
    .AddCorsServices(builder.Configuration)
    .AddSwaggerServices()
    .AddAuthenticationServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddApplicationHealthChecks(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseApplicationPipeline(builder.Environment);

// Map endpoints
app.MapControllers();
app.MapApplicationHealthChecks();

// Apply database migrations
app.ApplyDatabaseMigrations(builder.Environment);

app.Run();
