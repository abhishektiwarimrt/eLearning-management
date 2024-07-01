namespace lms.usermanagement.api.Profiles.GetProfiles
{
    //public record GetUsersProfileRequest(int? PageNumber = 1, int? PageSize = 10);
    public record GetUsersProfileResponse(IEnumerable<UserProfile> UsersProfile);
    public class GetUsersProfileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/UsersProfile",
            async (ISender sender) =>
            {
                var result = await sender.Send(new GetUsersProfileQuery());

                var response = result.Adapt<GetUsersProfileResponse>();

                return Results.Ok(response);

            })
            .WithName("GetUsersProfile")
            .Produces<GetUsersProfileResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Users Profile")
            .WithDescription("Get Users Profile");
        }
    }
}
