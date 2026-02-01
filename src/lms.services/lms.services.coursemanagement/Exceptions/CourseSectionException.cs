
namespace lms.services.coursemanagement.Exceptions
{
    public class CourseSectionException : InternalServerException
    {
        public CourseSectionException(string message) : base(message)
        {
        }

        public CourseSectionException(string message, string details) : base(message)
        {
            CourseCreationFailedDetails = details;
        }

        public string? CourseCreationFailedDetails { get; }
    }
}
