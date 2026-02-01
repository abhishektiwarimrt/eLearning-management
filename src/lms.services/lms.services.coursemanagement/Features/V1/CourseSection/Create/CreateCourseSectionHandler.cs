namespace lms.services.coursemanagement.Features.V1.CourseSection.Create
{
    public record CreateCourseSectionsCommand(IList<CourseSectionDto> CourseSectionDtos)
       : ICommand<CreateCourseSectionsResult>
    {
        public Guid CourseId { get; set; }
    }
    public record CreateCourseSectionsResult(bool CourseSectionsCreated);

    public class CreateCourseSectionHandler(
        ICourseSectionService courseSectionService
        , IUnitOfWork<CourseDbContext> unitOfWork
        , ILogger<CreateCourseSectionHandler> logger)
        : ICommandHandler<CreateCourseSectionsCommand, CreateCourseSectionsResult>
    {
        public async Task<CreateCourseSectionsResult> Handle(CreateCourseSectionsCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var courseSectionCreated = false;
                await unitOfWork.BeginTransactionAsync();

                var addedCourseDto = await courseSectionService.CreateCourseSectionsAsync(command.CourseId, command.CourseSectionDtos);
                if (addedCourseDto != null)
                {
                    courseSectionCreated = true;
                }

                await unitOfWork.CommitAsync();
                return new CreateCourseSectionsResult(courseSectionCreated);
            }
            catch (Exception ex)
            {

                await unitOfWork.RollbackAsync();
                var courseSectionsStr = string.Join(" ", command.CourseSectionDtos.Select(x => x.Title));
                logger.LogError(ex, "Failed to add course serction:[{CourseSectionsStr}] to CourseId: {CourseId}\n{Exception}", courseSectionsStr, command.CourseId, ex);

                throw;
            }

        }
    }
}
