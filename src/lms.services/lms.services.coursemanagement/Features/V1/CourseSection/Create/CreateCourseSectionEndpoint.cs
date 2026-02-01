

namespace lms.services.coursemanagement.Features.V1.CourseSection.Create
{
    public record CreateCourseSectionsRequest(IList<CourseSectionDto> CourseSectionDtos);
    public record CreateCourseSectionsResponse(bool CourseSectionsCreated);

    public class CreateCourseSectionEndpoint : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new(1, 0);
        protected override string ApiName => "Courses";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            group.MapPost("/{CourseId}/Sections",
               async (Guid CourseId, CreateCourseSectionsRequest request, ISender sender, HttpContext context) =>
               {

                   var courseIdObj = context.Request.RouteValues["CourseId"];
                   if (courseIdObj == null)
                   {
                       return Results.BadRequest("Invalid CourseId!");
                   }
                   var isValid = Guid.TryParse(courseIdObj.ToString(), out var courseId);
                   if (!isValid)
                   {
                       return Results.BadRequest("Invalid CourseId!");
                   }

                   var createCommand = request.Adapt<CreateCourseSectionsCommand>();
                   createCommand.CourseId = courseId;

                   var result = await sender.Send(createCommand);

                   var response = result.Adapt<CreateCourseSectionsResponse>();
                   var apiResponse = new ApiResponse<CreateCourseSectionsResponse>
                   {
                       Status = "success",
                       Data = response,
                       Metadata = new Metadata
                       {
                           Timestamp = DateTime.UtcNow,
                           Version = ApiVersion.ToString()
                       }
                   };

                   return Results.Created($"/{ApiName}/{response.CourseSectionsCreated}", apiResponse);

               })
               .MapToApiVersion(1, 0)
               .Produces<CreateCourseSectionsResponse>(StatusCodes.Status201Created)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .WithSummary("Create Course Sections")
               .WithDescription("Create Course Sections");
        }
    }
}
