using System.ComponentModel.DataAnnotations;

namespace lms.shared.data.entities.instructormanagement
{
    public class InstructorProfile
    {
        public int Id { get; set; }

        public string? ApplicationUserId { get; set; }  // FK to AspNetUsers.Id

        [StringLength(100)]
        public string? Headline { get; set; }

        [StringLength(2000)]
        public string? Biography { get; set; }

        [StringLength(10)]
        public string? PrimaryLanguage { get; set; } = "en";

        [StringLength(500)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Twitter { get; set; }

        [StringLength(100)]
        public string? Facebook { get; set; }

        [StringLength(100)]
        public string? LinkedIn { get; set; }

        [StringLength(100)]
        public string? YouTube { get; set; }

        public string? ProfileImageUrl { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
