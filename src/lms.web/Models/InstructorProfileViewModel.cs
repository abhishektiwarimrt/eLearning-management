using System.ComponentModel.DataAnnotations;

namespace lms.web.Models
{
    public class InstructorProfileViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [MaxLength(60, ErrorMessage = "Headline must be 60 characters or less")]
        public string? Headline { get; set; }

        [MaxLength(2000)]
        public string? Bio { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? Website { get; set; }

        [Url(ErrorMessage = "Please enter a valid LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        public string? TwitterHandle { get; set; }

        public string Language { get; set; } = "English";

        // UI state
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
