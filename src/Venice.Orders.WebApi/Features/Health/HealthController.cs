using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Venice.Orders.WebApi.Common;

namespace Venice.Orders.WebApi.Features.Health;

[ApiController]
[Route("api/[controller]")]
public class HealthController : BaseController
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Obtém o status de saúde completo da aplicação
    /// </summary>
    /// <returns>Status detalhado de todos os health checks</returns>
    [HttpGet("status")]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 200)]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 503)]
    public async Task<IActionResult> GetStatus()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        
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

        if (report.Status == HealthStatus.Healthy)
        {
            return Ok(new ApiResponseWithData<object>
            {
                Success = true,
                Message = "Application is healthy",
                Data = result
            });
        }

        return StatusCode(503, new ApiResponseWithData<object>
        {
            Success = false,
            Message = "Application is unhealthy",
            Data = result
        });
    }

    /// <summary>
    /// Obtém informações básicas da aplicação
    /// </summary>
    /// <returns>Informações da aplicação</returns>
    [HttpGet("info")]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 200)]
    public IActionResult GetInfo()
    {
        var info = new
        {
            application = "Venice Orders API",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            timestamp = DateTime.UtcNow,
            uptime = Environment.TickCount64,
            framework = Environment.Version.ToString(),
            os = Environment.OSVersion.ToString(),
            processorCount = Environment.ProcessorCount,
            workingSet = Environment.WorkingSet,
            gcMemoryInfo = GC.GetGCMemoryInfo()
        };

        return Ok(new ApiResponseWithData<object>
        {
            Success = true,
            Message = "Application information retrieved successfully",
            Data = info
        });
    }

    /// <summary>
    /// Verifica se a aplicação está pronta para receber tráfego
    /// </summary>
    /// <returns>Status de readiness</returns>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 200)]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 503)]
    public async Task<IActionResult> GetReady()
    {
        var report = await _healthCheckService.CheckHealthAsync(
            predicate: check => check.Tags.Contains("external") || check.Tags.Contains("database"));

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

        if (report.Status == HealthStatus.Healthy)
        {
            return Ok(new ApiResponseWithData<object>
            {
                Success = true,
                Message = "Application is ready",
                Data = result
            });
        }

        return StatusCode(503, new ApiResponseWithData<object>
        {
            Success = false,
            Message = "Application is not ready",
            Data = result
        });
    }

    /// <summary>
    /// Verifica se a aplicação está viva
    /// </summary>
    /// <returns>Status de liveness</returns>
    [HttpGet("live")]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 200)]
    [ProducesResponseType(typeof(ApiResponseWithData<object>), 503)]
    public async Task<IActionResult> GetLive()
    {
        var report = await _healthCheckService.CheckHealthAsync(
            predicate: check => check.Tags.Contains("app"));

        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration
        };

        if (report.Status == HealthStatus.Healthy)
        {
            return Ok(new ApiResponseWithData<object>
            {
                Success = true,
                Message = "Application is alive",
                Data = result
            });
        }

        return StatusCode(503, new ApiResponseWithData<object>
        {
            Success = false,
            Message = "Application is not alive",
            Data = result
        });
    }
}
