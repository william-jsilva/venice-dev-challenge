using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using RabbitMQ.Client;
using MongoDB.Driver;

namespace Venice.Orders.WebApi.HealthChecks;

public class ExternalServicesHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IConnection _rabbitMq;
    private readonly IMongoClient _mongoClient;
    private readonly string _sqlServerConnectionString;

    public ExternalServicesHealthCheck(
        IConnectionMultiplexer redis,
        IConnection rabbitMq,
        IMongoClient mongoClient,
        IConfiguration configuration)
    {
        _redis = redis;
        _rabbitMq = rabbitMq;
        _mongoClient = mongoClient;
        _sqlServerConnectionString = configuration.GetConnectionString("SqlServer") ?? string.Empty;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var healthData = new Dictionary<string, object>();
        var failures = new List<string>();

        // Verificar Redis
        try
        {
            var redisDb = _redis.GetDatabase();
            await redisDb.PingAsync();
            healthData["redis"] = "Healthy";
        }
        catch (Exception ex)
        {
            healthData["redis"] = "Unhealthy";
            failures.Add($"Redis: {ex.Message}");
        }

        // Verificar RabbitMQ
        try
        {
            if (_rabbitMq != null && _rabbitMq.IsOpen)
            {
                healthData["rabbitmq"] = "Healthy";
            }
            else
            {
                healthData["rabbitmq"] = "Unhealthy";
                failures.Add("RabbitMQ: Connection is not open");
            }
        }
        catch (Exception ex)
        {
            healthData["rabbitmq"] = "Unhealthy";
            failures.Add($"RabbitMQ: {ex.Message}");
        }

        // Verificar MongoDB
        try
        {
            await _mongoClient.ListDatabaseNamesAsync(cancellationToken: cancellationToken);
            healthData["mongodb"] = "Healthy";
        }
        catch (Exception ex)
        {
            healthData["mongodb"] = "Unhealthy";
            failures.Add($"MongoDB: {ex.Message}");
        }

        // Verificar SQL Server
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(_sqlServerConnectionString);
            await connection.OpenAsync(cancellationToken);
            healthData["sqlserver"] = "Healthy";
        }
        catch (Exception ex)
        {
            healthData["sqlserver"] = "Unhealthy";
            failures.Add($"SQL Server: {ex.Message}");
        }

        healthData["timestamp"] = DateTime.UtcNow;

        if (failures.Any())
        {
            return HealthCheckResult.Unhealthy(
                "Some external services are unhealthy",
                data: healthData);
        }

        return HealthCheckResult.Healthy("All external services are healthy", healthData);
    }
}
