
using lms.usermanagement.api.Profiles.CreateProfile;

namespace lms.usermanagement.api.Profiles.GetProfileById
{
    public record GetUserProfileByIdResponse(UserProfile UserProfile);
    public class GetUserProfileByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/UsersProfile/{id}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(new GetUserProfileByIdQuery(id));
                    var response = result.Adapt<GetUserProfileByIdResponse>();

                    return Results.Ok(response);
                })
                .WithName("GetUserProfileById")
                .Produces<CreateUserProfileResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get User Prfile By ID")
                .WithDescription("Get User Prfile By ID");
        }
    }
}
