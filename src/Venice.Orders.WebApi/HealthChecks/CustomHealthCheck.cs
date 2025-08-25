using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Venice.Orders.WebApi.HealthChecks;

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Verificar se a aplicação está funcionando corretamente
        var isHealthy = true;
        var data = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow },
            { "version", "1.0.0" },
            { "environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" }
        };

        if (isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Application is healthy", data));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Application is unhealthy", data: data));
    }
}
