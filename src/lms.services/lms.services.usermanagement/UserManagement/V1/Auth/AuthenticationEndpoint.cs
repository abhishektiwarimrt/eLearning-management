using lms.buildingblocks.RequestResponse;
using lms.services.usermanagement.UserManagement.V1.AddRole;

namespace lms.services.usermanagement.UserManagement.V1.Auth
{
    public record UserAuthRequest(LoginUserDto UserLogin);
    
    public class AuthenticationEndpoint(ILogger<AddUserRoleEndpoint> logger) : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new ApiVersion(1, 0);

        protected override string ApiName => "Auth";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            logger.LogInformation("");
            group.MapPost("/Login",
                async (UserAuthRequest request, ISender sender, HttpContext context) =>
                {
                    var command = request.Adapt<UserAuthCommand>();
                    var result = await sender.Send(command);

                    var apiResponse = new ApiResponse<UserAuthResponse>
                    {
                        Status = "error",
                        Data = null,
                        Metadata = new Metadata
                        {
                            Timestamp = DateTime.UtcNow,
                            Version = ApiVersion.ToString()
                        }
                    };
                    if (result.Success)
                    {
                        apiResponse.Status = "success";
                        apiResponse.Data = result;
                        //var apiResponse = new ApiResponse<UserAuthResponse>
                        //{
                        //    Status = "success",
                        //    Data = result,
                        //    Metadata = new Metadata
                        //    {
                        //        Timestamp = DateTime.UtcNow,
                        //        Version = ApiVersion.ToString()
                        //    }
                        //};
                        return Results.Ok(apiResponse);
                    }
                    else
                    {
                        apiResponse.Status = "error";
                        apiResponse.Data = null;
                        //var apiResponse = new ApiResponse<UserAuthResponse>
                        //{
                        //    Status = "error",
                        //    Data = null,
                        //    Metadata = new Metadata
                        //    {
                        //        Timestamp = DateTime.UtcNow,
                        //        Version = ApiVersion.ToString()
                        //    }
                        //};
                        return Results.BadRequest(apiResponse);
                    }

                })
                .MapToApiVersion(1, 0)
                .Produces<AddUserRoleResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Authenticate User Credentials")
                .WithDescription("Authenticate User Credentials");
        }
    }
}
