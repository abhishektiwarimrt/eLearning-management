using lms.shared.data.entities.coursemanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.shared.data.entities.coursemanagement.Content
{
    public class FileUploadQueueItem
    {
        public Guid Id { get; set; }
        public Guid CourseModuleId { get; set; }
        public CourseModule? CourseModule { get; set; }  
        public string FileName { get; set; } = string.Empty;
        public byte[]? FileBytes { get; set; }
        public string QueueStatus { get; set; } = "Pending";
        public int RetryCount { get; set; } = 0;
        public DateTime QueuedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }

}
