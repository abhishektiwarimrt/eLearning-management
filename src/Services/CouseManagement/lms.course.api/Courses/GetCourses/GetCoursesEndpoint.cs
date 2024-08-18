

using Mapster;

namespace lms.course.api.Courses.GetCourses
{
    //public record GetCoursesRequest(Guid UserProfileId) : IRequest<GetCoursesResponse>;
    public record GetCoursesResponse(IEnumerable<Course> Courses);
    public class GetCoursesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Courses/{userProfileId}",
                async (Guid userProfileId, ISender sender) =>
                {
                    var result = await sender.Send(new GetCoursesQuery(userProfileId));

                    var response = result.Adapt<GetCoursesResponse>();

                    return Results.Ok(response);
                })
                .WithName("GetCourses")
                .Produces<GetCoursesResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Users Courses")
                .WithDescription("Get Users Course");
        }
    }
}
