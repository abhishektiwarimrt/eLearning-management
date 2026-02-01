using Carter;
using Microsoft.AspNetCore.Builder;

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

            // Add default services           
            builder.Services.AddCarter();
            builder.Services.AddVersionedApi();

            // Allow additional service configuration
            configureServices?.Invoke(builder);

            var app = builder.Build();

            // Add default middleware
            app.MapCarter();

            // Allow additional app configuration
            configureApp?.Invoke(app);

            return app;
        }
    }
}
