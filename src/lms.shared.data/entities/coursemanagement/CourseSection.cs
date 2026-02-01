namespace lms.shared.data.entities.coursemanagement
{
    public class CourseSection
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Course Course { get; set; } = null!;
        public List<CourseModule> CourseModules { get; set; } = [];
    }
}