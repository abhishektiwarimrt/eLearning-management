namespace lms.web.Models
{
    public class UserRolesViewModel
    {
        public required string UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IList<string>? CurrentRoles { get; set; } 
        public IList<string>? AvailableRoles { get; set; }
    }
}
