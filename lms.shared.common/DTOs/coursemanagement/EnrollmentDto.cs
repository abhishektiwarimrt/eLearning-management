namespace lms.shared.common.DTOs.coursemanagement
{
    public class EnrollmentDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
        public Guid CourseId { get; set; }
        public required string CourseTitle { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
