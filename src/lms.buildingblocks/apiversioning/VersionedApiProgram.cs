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
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            // Add service defaults & Aspire client integrations.
            _ = builder.AddServiceDefaults();
            // Add default services           
            _ = builder.Services.AddCarter();
            _ = builder.Services.AddVersionedApi();

            // Allow additional service configuration
            configureServices?.Invoke(builder);

            WebApplication app = builder.Build();
            _ = app.MapDefaultEndpoints();
            // Add default middleware
            _ = app.MapCarter();

            // Allow additional app configuration
            configureApp?.Invoke(app);

            return app;
        }
    }
}
