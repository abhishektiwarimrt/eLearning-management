
namespace lms.services.coursemanagement.Exceptions
{
    public class CourseException : InternalServerException
    {
        public CourseException(string message) : base(message)
        {
        }

        public CourseException(string message, string details) : base(message)
        {
            CourseCreationFailedDetails = details;
        }

        public string? CourseCreationFailedDetails { get; }
    }
}
