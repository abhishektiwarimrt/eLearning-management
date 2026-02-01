using lms.shared.data.entities.usermanagement;

namespace lms.services.usermanagement.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        //Task<AuthResult> RefreshTokenAsync(string refreshToken);
        //Task<bool> LogoutAsync(string userId);
        //Task<bool> ConfirmEmailAsync(string userId, string token);
        //Task<bool> SendVerificationEmailAsync(string userId);
        //Task<bool> IsEmailVerifiedAsync(string userId);
    }
}

