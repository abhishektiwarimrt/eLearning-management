using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.entities.usermanagement
{
    public class Role : IdentityRole<string>
    {
        public required override string Id { get; set; }
        public required override string Name { get; set; }
    }
}
