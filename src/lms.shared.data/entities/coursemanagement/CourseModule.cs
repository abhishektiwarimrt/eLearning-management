using lms.shared.data.entities.coursemanagement.Content;

namespace lms.shared.data.entities.coursemanagement
{
    public class CourseModule
    {
        public Guid Id { get; set; }
        public Guid CourseSectionId { get; set; }
        public required string Title { get; set; }
        public required CourseContentType ContentType { get; set; }
        public string? ContentReference { get; set; } // S3 key for documents and videos, or null for quizzes       
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Uploaded { get; set; } = false;
        public DateTime? UploadedAt { get; set; }
        public CourseSection CourseSection { get; set; } = null!;
        public IList<Quiz>? Quizes { get; set; }
        public ICollection<FileUploadQueueItem>? FileUploadQueueItems { get; set; }
    }
}
