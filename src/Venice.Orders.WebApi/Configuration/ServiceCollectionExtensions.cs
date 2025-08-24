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

namespace Venice.Orders.WebApi.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework - SQL Server
        services.AddDbContext<VeniceOrdersContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        // MongoDB
        services.AddSingleton<IMongoClient>(provider =>
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            return new MongoClient(settings);
        });

        services.AddSingleton<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDB:DatabaseName"] ?? "VeniceOrders";
            return client.GetDatabase(databaseName);
        });

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Redis connection string is not configured");
            }
            return ConnectionMultiplexer.Connect(connectionString);
        });

        // RabbitMQ
        services.AddSingleton<IConnection>(provider =>
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("RabbitMQ connection string is not configured");
            }
            var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
            return factory.CreateConnection();
        });

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationLayer).Assembly));

        // AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly, typeof(ApplicationLayer).Assembly);

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "VeniceOrders",
                    ValidAudience = configuration["Jwt:Audience"] ?? "VeniceOrders",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "your-secret-key-here"))
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
                var allowedMethods = configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? new[] { "*" };
                var allowedHeaders = configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? new[] { "*" };

                if (allowedOrigins.Contains("*"))
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins);
                }

                if (allowedMethods.Contains("*"))
                {
                    policy.WithMethods(allowedMethods);
                }
                else
                {
                    policy.AllowAnyMethod();
                }

                if (allowedHeaders.Contains("*"))
                {
                    policy.AllowAnyHeader();
                }
                else
                {
                    policy.WithHeaders(allowedHeaders);
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
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

        return services;
    }
}
