namespace lms.shared.common.DTOs.usermanagement
{
    public class ExternalLoginInfoDto
    {
        public required string LoginProvider { get; set; }
        public required string ProviderKey { get; set; }
        public required string ProviderDisplayName { get; set; }
        public required UserDto User { get; set; }
    }
}
