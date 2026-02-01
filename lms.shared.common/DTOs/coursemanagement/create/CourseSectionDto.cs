namespace lms.shared.common.DTOs.coursemanagement.create
{
    public class CourseSectionDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}
