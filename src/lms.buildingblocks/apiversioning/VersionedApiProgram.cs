using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace lms.buildingblocks.apiversioning
{
    // 4. Create a base Program class for common setup
    public abstract class VersionedApiProgram
    {
        protected static WebApplication ConfigureApi(string[] args,
            Action<WebApplicationBuilder>? configureServices = null,
            Action<WebApplication>? configureApp = null)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Wire up Aspire service defaults: OpenTelemetry, health checks, service discovery
            builder.AddServiceDefaults();

            // Add default services
            builder.Services.AddCarter();
            builder.Services.AddVersionedApi();

            // Allow additional service configuration
            configureServices?.Invoke(builder);

            var app = builder.Build();

            // Expose /health and /alive endpoints
            app.MapDefaultEndpoints();

            // Add default middleware
            app.MapCarter();

            // Allow additional app configuration
            configureApp?.Invoke(app);

            return app;
        }
    }
}
