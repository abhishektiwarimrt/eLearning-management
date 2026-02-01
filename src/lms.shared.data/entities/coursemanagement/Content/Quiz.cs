namespace lms.shared.data.entities.coursemanagement.Content
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public Guid CourseModuleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Question> Questions { get; set; } = [];
        public CourseModule CourseModule { get; set; } = null!;
    }
}
