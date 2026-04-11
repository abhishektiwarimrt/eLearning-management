using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace lms.buildingblocks.OpenAPI
{
    public class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        }

        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            // ── JWT Bearer security definition (shared across all versions) ──
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Enter your JWT token below. Obtain it from POST /api/v1/Auth/Login.\n\nExample: eyJhbGci..."
            });

            // Apply Bearer security to every operation via a document filter (runs after doc is fully built)
            options.DocumentFilter<BearerSecurityDocumentFilter>();

            foreach (ApiVersionDescription description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                // Use the entry assembly name so each application/project shows its own title/description
                string appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "API";

                OpenApiInfo openApiInfo = new OpenApiInfo
                {
                    Title = $"{appName} API v{description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = $"{appName} - Microservices for {appName} - {description.ApiVersion}",
                    Contact = new OpenApiContact
                    {
                        Name = "Abhishek Tiwari",
                        Email = "abhishektiwarimrt@gmail.com",
                        Url = new Uri("https://abhishek-tiwari-g3r1v88.gamma.site/"),
                    }
                };

                options.SwaggerDoc(description.GroupName, openApiInfo);
                options.OperationFilter<SwaggerFileOperationFilter>();
            }
        }
    }
}