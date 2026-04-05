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
            builder.AddServiceDefaults();
            // Add default services           
            builder.Services.AddCarter();
            builder.Services.AddVersionedApi();

            // Allow additional service configuration
            configureServices?.Invoke(builder);

            WebApplication app = builder.Build();
            app.MapDefaultEndpoints();
            // Add default middleware
            app.MapCarter();

            // Allow additional app configuration
            configureApp?.Invoke(app);

            return app;
        }
    }
}
