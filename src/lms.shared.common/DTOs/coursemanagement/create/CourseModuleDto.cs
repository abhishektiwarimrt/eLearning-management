using lms.shared.common.utilities;
using Microsoft.AspNetCore.Http;

namespace lms.shared.common.DTOs.coursemanagement.create
{
    public class CourseModuleDto
    {
        public string? Title { get; set; } = string.Empty;
        public CourseContentType ContentType { get; set; }
        public IFormFile? File { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool FileUploaded { get; set; }
    }
}