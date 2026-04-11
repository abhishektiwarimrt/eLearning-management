using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace lms.buildingblocks.OpenAPI
{
    /// <summary>
    /// Adds Bearer JWT security to the global security section and each operation in the Swagger document.
    /// Uses a DocumentFilter so it runs after the document is fully assembled.
    /// </summary>
    public class BearerSecurityDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Resolve the Bearer scheme from the already-registered components
            if (swaggerDoc.Components?.SecuritySchemes is null ||
                !swaggerDoc.Components.SecuritySchemes.TryGetValue("Bearer", out var bearerScheme))
                return;

            // Create a reference that resolves against the host document
            var bearerRef = new OpenApiSecuritySchemeReference("Bearer", swaggerDoc);

            var requirement = new OpenApiSecurityRequirement
            {
                { bearerRef, new List<string>() }
            };

            // Global security requirement on the document
            swaggerDoc.Security ??= new List<OpenApiSecurityRequirement>();
            swaggerDoc.Security.Clear();
            swaggerDoc.Security.Add(requirement);

            // Per-operation lock icon
            if (swaggerDoc.Paths is null) return;
            foreach (var path in swaggerDoc.Paths.Values)
            {
                if (path.Operations is null) continue;
                foreach (var operation in path.Operations.Values)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    if (operation.Security.Count == 0)
                        operation.Security.Add(requirement);
                }
            }
        }
    }
}
