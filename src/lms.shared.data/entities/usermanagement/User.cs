using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.entities.usermanagement
{
    public class User : IdentityUser<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? Headline { get; set; }   // max 60 chars, shown on course cards
        public string? Bio { get; set; }   // max 2000 chars, shown on profile page
        public string? Website { get; set; }   // full URL
        public string? LinkedInUrl { get; set; }   // full URL
        public string? TwitterHandle { get; set; }   // handle only, no @
        public string? Language { get; set; }   // primary teaching language
    }
}
