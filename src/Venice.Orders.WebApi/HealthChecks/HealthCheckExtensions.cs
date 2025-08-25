using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using StackExchange.Redis;
using Venice.Orders.Infrastructure.Data;
using Venice.Orders.WebApi.HealthChecks;

namespace Venice.Orders.WebApi.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck<CustomHealthCheck>("application", tags: new[] { "app" })
            .AddCheck<ExternalServicesHealthCheck>("external_services", tags: new[] { "external" })
            .AddSqlServer(
                configuration.GetConnectionString("SqlServer") ?? string.Empty,
                name: "sqlserver",
                tags: new[] { "database", "sql" },
                timeout: TimeSpan.FromSeconds(5))
            .AddMongoDb(
                provider => 
                {
                    var connectionString = configuration.GetConnectionString("MongoDB") ?? string.Empty;
                    var settings = MongoClientSettings.FromConnectionString(connectionString);
                    return new MongoClient(settings);
                },
                name: "mongodb",
                tags: new[] { "database", "nosql" },
                timeout: TimeSpan.FromSeconds(5))
            .AddRedis(
                configuration.GetConnectionString("Redis") ?? string.Empty,
                name: "redis",
                tags: new[] { "cache" },
                timeout: TimeSpan.FromSeconds(5))
            .AddDbContextCheck<VeniceOrdersContext>(
                name: "ef_core",
                tags: new[] { "database", "ef" });

        return services;
    }

    public static IEndpointRouteBuilder MapApplicationHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        // Health Check endpoints
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration,
                        tags = e.Value.Tags,
                        data = e.Value.Data
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("external") || check.Tags.Contains("database"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("app"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    duration = report.TotalDuration
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });

        return endpoints;
    }
}
