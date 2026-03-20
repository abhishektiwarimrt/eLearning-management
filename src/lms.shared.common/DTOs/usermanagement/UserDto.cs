namespace lms.shared.common.DTOs.usermanagement
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // ── Instructor profile fields (NEW) ──────────────────────────────────
        // These are nullable — non-instructors will simply have null values.
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public string? Website { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TwitterHandle { get; set; }
        public string? Language { get; set; }
    }
}
