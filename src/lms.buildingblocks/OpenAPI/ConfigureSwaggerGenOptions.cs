using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                var openApiInfo = new OpenApiInfo
                {
                    Title = $"eLearning Management API v{description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = $"Microservices for eLearning Management - {description.ApiVersion}",
                    Contact = new OpenApiContact
                    {
                        Name = "Abhishek Tiwari",
                        Email = "abhishektiwarimrt@gmail.com",
                        Url = new Uri("https://yourwebsite.com"),
                    }
                };

                options.SwaggerDoc(description.GroupName, openApiInfo);
                //options.OperationFilter<SwaggerFileOperationFilter>();
                //options.AddSecurityDefinition("multipart/form-data",
                //   new OpenApiSecurityScheme
                //   {
                //       Type = SecuritySchemeType.ApiKey,
                //       In = ParameterLocation.Header,
                //       Name = "Content-Type",
                //       Description = "Upload files using multipart/form-data"
                //   });

            }
        }
    }
}
