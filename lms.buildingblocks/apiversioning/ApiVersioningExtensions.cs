using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace lms.buildingblocks.apiversioning
{
    // 3. Create an extension method for consistent service configuration
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddVersionedApi(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("x-api-version"),
                    new QueryStringApiVersionReader("api-version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
