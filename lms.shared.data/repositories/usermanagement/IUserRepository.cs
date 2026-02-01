using lms.shared.data.entities.usermanagement;
using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.repositories.usermanagement
{
    public interface IUserRepository
    {
        // Basic CRUD operations
        Task<IdentityResult?> CreateAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        //Task<UserDto> GetByIdAsync(int id);


        //Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto);
        //Task DeleteAsync(int id);

        //// User profile operations
        //Task<UserProfileDto> GetProfileAsync(int userId);
        //Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);

        //// Search and filtering
        //Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, int page, int pageSize);
        //Task<IEnumerable<UserDto>> GetByRoleAsync(string roleName, int page, int pageSize);

        //// Authentication and security
        //Task<bool> ValidateCredentialsAsync(string email, string password);
        //Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        //Task<bool> ResetPasswordAsync(string email);

        //// Roles and permissions
        //Task<IEnumerable<string>> GetRolesAsync(int userId);
        //Task<bool> AddToRoleAsync(int userId, string roleName);
        //Task<bool> RemoveFromRoleAsync(int userId, string roleName);
        //Task<bool> HasPermissionAsync(int userId, string permission);

        //// User activity and statistics
        //Task<DateTime?> GetLastLoginDateAsync(int userId);
        //Task UpdateLastLoginDateAsync(int userId);
        //Task<int> GetCourseCountAsync(int userId);

        //// Account verification and status
        //Task<bool> IsEmailVerifiedAsync(int userId);
        //Task<bool> SetEmailVerifiedAsync(int userId);
        //Task<bool> LockAccountAsync(int userId);
        //Task<bool> UnlockAccountAsync(int userId);

        //// External login
        //Task<UserDto> CreateOrUpdateFromExternalLoginAsync(ExternalLoginInfo externalLoginInfo);
        //Task<bool> LinkExternalLoginAsync(int userId, ExternalLoginInfo externalLoginInfo);

        //// Notification preferences
        //Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(int userId);
        //Task<NotificationPreferencesDto> UpdateNotificationPreferencesAsync(int userId, UpdateNotificationPreferencesDto preferencesDto);

        //// Data export and GDPR compliance
        //Task<UserDataExportDto> ExportUserDataAsync(int userId);
        //Task<bool> MarkForDeletionAsync(int userId);
    }
}

