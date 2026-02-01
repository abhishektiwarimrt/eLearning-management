namespace lms.shared.common.DTOs.usermanagement
{
    public class UserDataExportDto
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required IEnumerable<string> Roles { get; set; }
    }
}
