namespace lms.shared.common.DTOs.coursemanagement.create
{
    public class CourseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatorId { get; set; }
        public IList<CourseSectionDto> Sections { get; set; } = [];
    }
}
