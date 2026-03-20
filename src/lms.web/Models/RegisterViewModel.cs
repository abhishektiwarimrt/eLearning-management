using System.ComponentModel.DataAnnotations;

namespace lms.web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = "";

        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
