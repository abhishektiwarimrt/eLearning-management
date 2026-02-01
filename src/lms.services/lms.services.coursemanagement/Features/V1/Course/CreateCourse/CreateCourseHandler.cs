namespace lms.services.coursemanagement.Features.V1.Course.CreateCourse
{
    public record CreateCourseCommand(CourseDto CourseDto)
        : ICommand<CreateCourseResult>;
    public record CreateCourseResult(bool Created);

    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.CourseDto).NotNull();
            RuleFor(x => x.CourseDto.CreatorId).Must(x => x > 0).WithMessage("CreatorId must be greater than 0");
            RuleFor(x => x.CourseDto.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.CourseDto.Description).NotEmpty().WithMessage("Description is required");
            RuleForEach(x => x.CourseDto.Sections).NotNull().SetValidator(new CourseSectionDtoValidator());
        }
    }

    public class CourseSectionDtoValidator : AbstractValidator<CourseSectionDto>
    {
        public CourseSectionDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Order).GreaterThan(0).WithMessage("Order must be greater than 0.");
        }
    }



    public class CreateCourseHandler(
        ICourseService courseService
        , IUnitOfWork<CourseDbContext> unitOfWork
        , ILogger<CreateCourseHandler> logger)
        : ICommandHandler<CreateCourseCommand, CreateCourseResult>
    {
        public async Task<CreateCourseResult> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var courseCreated = false;
                await unitOfWork.BeginTransactionAsync();

                var addedCourseDto = await courseService.CreateCourseAsync(command.CourseDto);
                if (addedCourseDto != null)
                {
                    courseCreated = true;
                }

                await unitOfWork.CommitAsync();
                return new CreateCourseResult(courseCreated);
            }
            catch (Exception ex)
            {

                await unitOfWork.RollbackAsync();
                logger.LogError(ex, "Failed to add course: {Roles} to user: {UserId}\n{Exception}", string.Join(", ", command.CourseDto.Title), command.CourseDto.CreatorId, ex);

                throw;
            }
        }
    }
}

