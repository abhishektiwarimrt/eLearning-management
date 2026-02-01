namespace lms.shared.data.entities.coursemanagement
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public Guid CourseId { get; set; }
        public required Course Course { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
