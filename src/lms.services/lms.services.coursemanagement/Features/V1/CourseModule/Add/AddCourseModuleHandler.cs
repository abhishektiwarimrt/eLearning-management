namespace lms.services.coursemanagement.Features.V1.CourseModule.Add
{
    public record CreateCourseModeulesCommand(IList<CourseModuleDto> CourseModules)
        : ICommand<CreateCourseModeulesCommandResult>
    {
        public Guid CourseId { get; set; }
        public Guid CourseSectionId { get; set; }
    }

    public record CreateCourseModeulesCommandResult(IList<CourseModuleDto> CourseModules);
    public class AddCourseModuleHandler(
        ICourseModuleService courseModuleService,
        ILogger<AddCourseModuleHandler> logger)
        : ICommandHandler<CreateCourseModeulesCommand, CreateCourseModeulesCommandResult>
    {
        public async Task<CreateCourseModeulesCommandResult> Handle(CreateCourseModeulesCommand command, CancellationToken cancellationToken)
        {
            try
            {
                string createMesage;


                var addedCourseModulesDto = await courseModuleService.CreateModuleAsync(command.CourseId, command.CourseSectionId, command.CourseModules);
                //if (addedCourseModulesDto != null && addedCourseModulesDto.)
                //{
                //    createMesage = true;
                //}


                return new CreateCourseModeulesCommandResult(addedCourseModulesDto);
            }
            catch (Exception ex)
            {
                var courseModulesStr = string.Join(" ,", command.CourseModules);
                var message = $"Failed to add course modules: {courseModulesStr} to Course Sections: {command.CourseSectionId} and Course {command.CourseId}";
                logger.LogError(ex, message);
                throw;
            }
        }
    }


    public class CreateCourseModeulesCommandValidator : AbstractValidator<CreateCourseModeulesCommand>
    {
        public CreateCourseModeulesCommandValidator()
        {
            RuleForEach(x => x.CourseModules).SetValidator(new CourseModuleDtoValidator());
        }
    }

    public class CourseModuleDtoValidator : AbstractValidator<CourseModuleDto>
    {
        private static readonly string[] AllowedExtensions = { ".docx", ".pdf", ".mp4" };
        public CourseModuleDtoValidator()
        {
            long maxFileSize = 4L * 1024L * 1024L * 1024L;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Matches(@"^[A-Za-z0-9()'""\,-_\ ]+$")
                .WithMessage("Invalid Module Title. Only alphabets, numbers, and spaces are allowed.");

            //RuleFor(x => x.File)
            //    .Must(file => file == null).WithMessage("File is required.")
            //    .Must(file => file != null && file.Length > 0).WithMessage("File cannot be empty.")
            //    .Must(file => file != null && file.Length <= maxFileSize).WithMessage("File size must be less than 4GB.")
            //    .Must(file => file != null && AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            //    .WithMessage("Only .docx, .pdf and .mp4 files are allowed.");

            RuleFor(x => x.File == null ? null : x.File.FileName)
                .NotEmpty().WithMessage("Title is required.")
                .Matches(@"^[\w\s\-,\""']+\.[A-Za-z0-9]{3,4}$")
                .WithMessage("Invalid File Name. Only alphabets, numbers, and spaces are allowed.");
        }
    }
}
