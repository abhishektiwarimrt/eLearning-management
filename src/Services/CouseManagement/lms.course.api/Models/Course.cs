namespace lms.course.api.Models
{
    public class Course
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Desription { get; set; }
        public Guid UserProfileId { get; set; }
    }
}
