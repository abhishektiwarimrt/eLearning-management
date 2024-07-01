namespace lms.usermanagement.api.Profiles.CreateProfile
{
    public record CreateUserProfileRequest(string FirstName, string LastName, string Title, string Address, string City, string Country);

    public record CreateUserProfileResponse(Guid Id);

    public class CreateUserProfileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/UsersProfile",
            async (CreateUserProfileRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateUserProfileCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CreateUserProfileResponse>();

                return Results.Created($"/UsersProfile/{response.Id}", response);

            })
            .WithName("CreateUserProfile")
            .Produces<CreateUserProfileResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
        }
    }
}
