using lms.buildingblocks.RequestResponse;
using lms.services.usermanagement.UserManagement.V1.GetUserByEmail;

namespace lms.services.usermanagement.UserManagement.V1.GetRoleByEmail
{

    public record GetUserRoleByEmailResponse(IList<string> UserRoles);
    public class GetRoleByEmailEndpoint : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new ApiVersion(1, 0);
        protected override string ApiName => "User";


        protected override void ConfigureApi(RouteGroupBuilder group)
        {

            group.MapGet("/{UserEmail}/Roles",
            async (string UserEmail, ISender sender) =>
            {
                var result = await sender.Send(new GetUserRoleByEmailQuery(UserEmail));

                var response = result.Adapt<GetUserRoleByEmailResponse>();

                var apiResponse = new ApiResponse<GetUserRoleByEmailResponse>
                {
                    Status = "success",
                    Data = response,
                    Metadata = new Metadata
                    {
                        Timestamp = DateTime.UtcNow,
                        Version = ApiVersion.ToString()
                    }

                };
                return Results.Ok(apiResponse);

            })
            .MapToApiVersion(1, 0)
            .Produces<GetUserByEmailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Roles By Email")
            .WithDescription("Get User roles Email");
        }
    }
}
