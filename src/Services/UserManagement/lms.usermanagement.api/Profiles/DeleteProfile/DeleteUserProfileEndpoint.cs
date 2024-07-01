namespace lms.usermanagement.api.Profiles.DeleteProfile
{
    public record DeleteUserProfileRequest(Guid Id);
    public record DeleteUserProfileResponse(bool isSuccess);
    public class DeleteUserProfileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/UsersProfile/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteUserProfileCommand(id));

                var response = result.Adapt<DeleteUserProfileResponse>();

                return Results.Ok(response);
            })
         .WithName("DeleteUserProfile")
         .Produces<DeleteUserProfileResponse>(StatusCodes.Status200OK)
         .ProducesProblem(StatusCodes.Status400BadRequest)
         .ProducesProblem(StatusCodes.Status404NotFound)
         .WithSummary("Delete User Profile")
         .WithDescription("Delete User Profile");
        }
    }
}
