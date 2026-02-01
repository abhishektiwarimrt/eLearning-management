using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace lms.buildingblocks.OpenAPI
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFileCollection))
            .ToList();

            if (fileParams.Any())
            {
                operation.Parameters.Clear();
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    },
                                    ["Title"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    },
                                    ["ContentType"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    },
                                    ["Content"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    },
                                    ["Order"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    }
                                },
                                Required = new HashSet<string> { "file" }
                            }
                        }
                    }
                };
            }
        }
    }
}