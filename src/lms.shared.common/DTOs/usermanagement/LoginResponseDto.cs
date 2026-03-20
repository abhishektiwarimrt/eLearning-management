namespace lms.shared.common.DTOs.usermanagement
{
    public class LoginResponseDto
    {
        bool Success;
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public DateTime Expires { get; set; }
    }
}
