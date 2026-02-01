namespace lms.shared.data.entities.usermanagement
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
