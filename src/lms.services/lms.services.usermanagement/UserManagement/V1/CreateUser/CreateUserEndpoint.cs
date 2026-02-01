using lms.buildingblocks.RequestResponse;

namespace lms.services.usermanagement.UserManagement.V1.CreateUser
{
    public record CreateUserRequest(RegisterUserDto RegisterUser);

    public record CreateUserProfileResponse(bool Registered);
    public class CreateUserEndpoint : VersionedCarterModule
    {

        protected override ApiVersion ApiVersion => new ApiVersion(1, 0);
        protected override string ApiName => "User";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {

            group.MapPost("/",
            async (CreateUserRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateUserCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CreateUserProfileResponse>();
                var apiResponse = new ApiResponse<CreateUserProfileResponse>
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
            .Produces<CreateUserProfileResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create User")
            .WithDescription("Create User");
        }
    }
}
