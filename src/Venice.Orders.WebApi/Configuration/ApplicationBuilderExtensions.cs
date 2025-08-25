using Venice.Orders.WebApi.Middleware;
using Venice.Orders.WebApi.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace Venice.Orders.WebApi.Configuration;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationPipeline(
        this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        // Configure the HTTP request pipeline.
        // Enable Swagger for both Development and Docker environments
        if (environment.IsDevelopment() || environment.EnvironmentName == "Docker")
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Venice Orders API v1");
                // Swagger UI will be available at /swagger
            });
        }

        // Add middleware
        app.UseMiddleware<ValidationExceptionMiddleware>();

        // Use CORS
        app.UseCors("AllowAll");

        // HTTPS redirection disabled for Docker environment

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static IApplicationBuilder ApplyDatabaseMigrations(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        // Apply migrations when running in Docker
        using (var scope = app.ApplicationServices.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<Venice.Orders.Infrastructure.Data.VeniceOrdersContext>();
                context.Database.Migrate();
                Console.WriteLine("Database migrations applied successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
            }
        }

        return app;
    }
}
