namespace lms.shared.common.DTOs.usermanagement
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
