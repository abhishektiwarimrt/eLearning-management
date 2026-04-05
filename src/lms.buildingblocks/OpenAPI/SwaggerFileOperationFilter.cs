using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace lms.buildingblocks.OpenAPI
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            List<ParameterInfo> fileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFileCollection))
                .ToList();

            if (fileParams.Any())
            {
                operation.Parameters.Clear();
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, IOpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Properties = new Dictionary<string, IOpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String,
                                        Format = "binary"
                                    },
                                    ["Title"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
                                    },
                                    ["ContentType"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
                                    },
                                    ["Content"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
                                    },
                                    ["Order"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
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