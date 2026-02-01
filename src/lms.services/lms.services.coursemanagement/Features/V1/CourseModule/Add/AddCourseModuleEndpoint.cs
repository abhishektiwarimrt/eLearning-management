using lms.shared.common.utilities;

namespace lms.services.coursemanagement.Features.V1.CourseModule.Add
{
    public record CreateCoureModeulesRequst(IList<CourseModuleDto> CourseModules);
    public record CoureModeulesCommandResponse(List<CourseModuleDto> CourseModules);

    public class AddCourseModuleEndpoint : VersionedCarterModule
    {
        protected override ApiVersion ApiVersion => new(1, 0);
        protected override string ApiName => "Courses";

        protected override void ConfigureApi(RouteGroupBuilder group)
        {
            _ = group.MapPost("/{CourseId}/Sections/{SectionId}/Modules",
                async (
                    Guid CourseID
                    , Guid SectionID
                    , ISender sender
                    , HttpRequest httpRequest
               ) =>
                {
                    try
                    {
                        if (!httpRequest.HasFormContentType)
                        {
                            return Results.BadRequest("Request must be multipart/form-data");
                        }
                        var context = httpRequest.HttpContext;

                        var courseIdObj = context.Request.RouteValues["CourseId"];
                        var courseSectionIdObj = context.Request.RouteValues["SectionId"];

                        if (courseIdObj == null)
                        {
                            return Results.BadRequest("Invalid CourseId!");
                        }
                        if (courseSectionIdObj == null)
                        {
                            return Results.BadRequest("Invalid CourseId!");
                        }
                        var validCourseId = Guid.TryParse(courseIdObj.ToString()!, out var CourseId);
                        var ValidcourseSectionId = Guid.TryParse(courseSectionIdObj.ToString()!, out var SectionId);
                        if (!validCourseId || !ValidcourseSectionId)
                        {
                            return Results.BadRequest("Invalid CourseId! or CourseSectionId!");
                        }

                        var form = await httpRequest.ReadFormAsync();
                        var files = form.Files;

                        var courseModules = new List<CourseModuleDto>();
                        for (int i = 0; i < files.Count; i++)
                        {
                            var courseModule = new CourseModuleDto
                            {
                                Title = form.TryGetValue($"Title[{i}]", out var title) ? title : string.Empty,
                                File = files[i],
                                ContentType = Utility.GetFileContentType(files[i])
                            };
                            courseModules.Add(courseModule);
                        }

                        CreateCourseModeulesCommand? createCommand = null;
                        try
                        {
                            createCommand = new CreateCourseModeulesCommand(courseModules)
                            {
                                CourseId = CourseId,
                                CourseSectionId = SectionId
                            };
                        }
                        catch (Exception ex)
                        {
                            return Results.BadRequest(ex.Message);
                        }



                        var result = await sender.Send(createCommand);

                        var response = result.Adapt<CoureModeulesCommandResponse>();
                        var apiResponse = new ApiResponse<CoureModeulesCommandResponse>
                        {
                            Status = "success",
                            Data = response,
                            Metadata = new Metadata
                            {
                                Timestamp = DateTime.UtcNow,
                                Version = ApiVersion.ToString()
                            }
                        };
                        return Results.Created($"/{ApiName}/{true}", apiResponse);
                    }
                    catch (Exception ex)
                    {
                        return Results.BadRequest(ex.Message);
                    }

                })
                .Accepts<CreateCoureModeulesRequst>("multipart/form-data")
                .MapToApiVersion(1, 0)
                .Produces<ApiResponse<CoureModeulesCommandResponse>>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Create Course Sections Modules")
                .WithDescription("Create Course Sections Modules");
        }
    }



}
