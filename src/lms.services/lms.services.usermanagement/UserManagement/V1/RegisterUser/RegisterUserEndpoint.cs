using lms.buildingblocks.RequestResponse;

namespace lms.services.usermanagement.UserManagement.V1.CreateUser
{
    public record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);

    public record RegisterUserProfileResponse(bool Registered);
    public class RegisterUserEndpoint : VersionedCarterModule
    {

        protected override ApiVersion ApiVersion => new ApiVersion(1, 0);
        protected override string ApiName => "User";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {

            group.MapPost("/",
            async (RegisterUserRequest request, ISender sender) =>
            {
                RegisterUserCommand command = request.Adapt<RegisterUserCommand>();

                AddUserRoleResult result = await sender.Send(command);

                RegisterUserProfileResponse response = result.Adapt<RegisterUserProfileResponse>();
                var apiResponse = new ApiResponse<RegisterUserProfileResponse>
                {
                    Status = "success",
                    Data = response,
                    Metadata = new Metadata
                    {
                        Timestamp = DateTime.UtcNow,
                        Version = ApiVersion.ToString()
                    }
                };

                return Results.Created($"/User/{response.Registered}", apiResponse);

            })
            .MapToApiVersion(1.0)
            .Produces<RegisterUserProfileResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Register User")
            .WithDescription("Register User");
        }
    }
}
