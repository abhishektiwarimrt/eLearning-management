
namespace lms.services.coursemanagement.Features.V1.Course.CreateCourse
{
    public record CreateCourseRequest(CourseDto CourseDto);
    public record CreateCourseResponse(bool Created);
    public class CreateCourseEndpoint : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new(1, 0);
        protected override string ApiName => "Courses";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            group.MapPost("/",
                async (CreateCourseRequest request, ISender sender, HttpContext context) =>
                {
                    var createCommand = request.Adapt<CreateCourseCommand>();
                    var result = await sender.Send(createCommand);

                    var response = result.Adapt<CreateCourseResponse>();
                    var apiResponse = new ApiResponse<CreateCourseResponse>
                    {
                        Status = "success",
                        Data = response,
                        Metadata = new Metadata
                        {
                            Timestamp = DateTime.UtcNow,
                            Version = "1.0"
                        }
                    };
                    return Results.Created($"/{ApiName}/{response.Created}", apiResponse);

                })
                .MapToApiVersion(1, 0)
                .Produces<ApiResponse<CreateCourseResponse>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Create Course")
                .WithDescription("Create Course");
        }
    }
}
