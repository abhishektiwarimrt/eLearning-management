using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.entities.usermanagement
{
    public class User : IdentityUser<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
