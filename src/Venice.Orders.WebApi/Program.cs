using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using AutoMapper;
using StackExchange.Redis;
using MongoDB.Driver;
using RabbitMQ.Client;
using Venice.Orders.Infrastructure.Data;
using Venice.Orders.Infrastructure.Repositories;
using Venice.Orders.Infrastructure.Services;
using Venice.Orders.Domain.Repositories;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Application;
using Venice.Orders.WebApi.Middleware;
using Venice.Orders.WebApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Venice Orders API",
        Version = "v1",
        Description = "API for managing orders in the Venice system"
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Custom schema ID resolver to avoid conflicts
    c.CustomSchemaIds(type =>
    {
        if (type.Name == "OrderItemResult")
        {
            return $"{type.Namespace?.Split('.').Last()}.{type.Name}";
        }
        return type.Name;
    });

    // Add JWT authentication support
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "VeniceOrders",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "VeniceOrders",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-secret-key-here"))
        };
    });

builder.Services.AddAuthorization();

// Entity Framework - SQL Server
builder.Services.AddDbContext<VeniceOrdersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// MongoDB
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(provider =>
{
    var client = provider.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "VeniceOrders";
    return client.GetDatabase(databaseName);
});

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Redis connection string is not configured");
    }
    return ConnectionMultiplexer.Connect(connectionString);
});

// RabbitMQ
builder.Services.AddSingleton<IConnection>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("RabbitMQ");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("RabbitMQ connection string is not configured");
    }
    var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
    return factory.CreateConnection();
});

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

// Services
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();



// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationLayer).Assembly));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("application", tags: new[] { "app" })
    .AddCheck<ExternalServicesHealthCheck>("external_services", tags: new[] { "external" })
    .AddSqlServer(
        builder.Configuration.GetConnectionString("SqlServer") ?? string.Empty,
        name: "sqlserver",
        tags: new[] { "database", "sql" },
        timeout: TimeSpan.FromSeconds(5))
    .AddMongoDb(
        provider => new MongoClient(builder.Configuration.GetConnectionString("MongoDB") ?? string.Empty),
        name: "mongodb",
        tags: new[] { "database", "nosql" },
        timeout: TimeSpan.FromSeconds(5))
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? string.Empty,
        name: "redis",
        tags: new[] { "cache" },
        timeout: TimeSpan.FromSeconds(5))
    .AddDbContextCheck<VeniceOrdersContext>(
        name: "ef_core",
        tags: new[] { "database", "ef" });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add middleware
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
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

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
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

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
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

// Apply migrations when running in Docker
if (app.Environment.EnvironmentName == "Docker")
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<VeniceOrdersContext>();
            context.Database.Migrate();
            Console.WriteLine("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex.Message}");
        }
    }
}

app.Run();
