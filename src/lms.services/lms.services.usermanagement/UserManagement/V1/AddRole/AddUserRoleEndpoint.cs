using lms.buildingblocks.RequestResponse;

namespace lms.services.usermanagement.UserManagement.V1.AddRole
{
    public record AddUserRoleRequest(IList<Roles> UserRoles);
    public record AddUserRoleResponse(bool RoleAdded);
    public class AddUserRoleEndpoint(ILogger<AddUserRoleEndpoint> logger) : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new ApiVersion(1, 0);
        protected override string ApiName => "User";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            logger.LogInformation("");
            group.MapPost("/{UserEmail}/Roles",
                async (string UserEmail, AddUserRoleRequest request, ISender sender, HttpContext context) =>
                {
                    var userEmail = context.Request.RouteValues["UserEmail"];

                    if (userEmail == null)
                    {
                        return Results.BadRequest("Invalid UserEmail!");
                    }

                    var command = request.Adapt<AddUserRoleCommand>() with { UserEmail = userEmail.ToString() };
                    var result = await sender.Send(command);

                    var response = result.Adapt<AddUserRoleResponse>();
                    var apiResponse = new ApiResponse<AddUserRoleResponse>
                    {
                        Status = "success",
                        Data = response,
                        Metadata = new Metadata
                        {
                            Timestamp = DateTime.UtcNow,
                            Version = ApiVersion.ToString()
                        }
                    };

                    return Results.Created($"/User/{response.RoleAdded}", apiResponse);

                })
                .MapToApiVersion(1, 0)
                .Produces<AddUserRoleResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Add User Roles")
                .WithDescription("Add User Roles");
        }
    }
}
