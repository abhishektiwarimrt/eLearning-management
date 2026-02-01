namespace lms.shared.data.entities.coursemanagement
{
    public class Course
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int CreatorId { get; set; }
        public List<CourseSection> Sections { get; set; } = [];
        public List<Enrollment> Enrollments { get; set; } = [];
    }
}
