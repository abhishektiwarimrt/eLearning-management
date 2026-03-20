using System.ComponentModel.DataAnnotations;

namespace lms.web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public DateTime? Expires { get; set; }
    }
}
