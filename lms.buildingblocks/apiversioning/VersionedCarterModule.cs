using Asp.Versioning;
using Asp.Versioning.Builder;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;


namespace lms.buildingblocks.apiversioning
{
    public abstract class VersionedCarterModule : ICarterModule
    {
        protected abstract ApiVersion ApiVersion { get; }
        protected abstract string ApiName { get; }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet(ApiName)
            .HasApiVersion(ApiVersion)
            .ReportApiVersions()
            .Build();

            RouteGroupBuilder group = app
            .MapGroup("api/v{version:apiVersion}/" + $"{ApiName.ToLowerInvariant()}")
            .WithApiVersionSet(apiVersionSet);

            ConfigureApi(group);
        }

        protected abstract void ConfigureApi(RouteGroupBuilder group);
    }
}
