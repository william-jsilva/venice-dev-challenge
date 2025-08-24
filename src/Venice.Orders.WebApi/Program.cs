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
using Venice.Orders.WebApi.Configuration;

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

// Apply database migrations
app.ApplyDatabaseMigrations(builder.Environment);

app.Run();
