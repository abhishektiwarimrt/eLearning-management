namespace lms.services.usermanagement.Services
{
    public interface IUserService
    {
        // User CRUD operations
        Task<bool?> CreateUserAsync(RegisterUserDto registerUserDto);
        Task<UserDto?> GetUserByEmailAsync(string email);
        //Task<UserDto> GetUserByIdAsync(int id);
        //Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        //Task DeleteUserAsync(int id);

        //// User profile management
        //Task<UserProfileDto> GetUserProfileAsync(int userId);
        //Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileDto updateProfileDto);

        //// User search and filtering
        //Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm, int page, int pageSize);

        //// Password management
        //Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        //Task<bool> ResetPasswordAsync(string email);

        //// User activity and statistics
        //Task<DateTime?> GetLastLoginDateAsync(int userId);
        //Task UpdateLastLoginDateAsync(int userId);
        //Task<int> GetUserCourseCountAsync(int userId);

        //// Account status
        //Task<bool> LockUserAccountAsync(int userId);
        //Task<bool> UnlockUserAccountAsync(int userId);

        //// Social media integration
        //Task<UserDto> CreateOrUpdateUserFromExternalLoginAsync(ExternalLoginInfo externalLoginInfo);
        //Task<bool> LinkExternalLoginAsync(int userId, ExternalLoginInfo externalLoginInfo);

        //// Notifications and preferences
        //Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(int userId);
        //Task<NotificationPreferencesDto> UpdateNotificationPreferencesAsync(int userId, UpdateNotificationPreferencesDto preferencesDto);

        //// User data export and GDPR compliance
        //Task<UserDataExportDto> ExportUserDataAsync(int userId);
        //Task<bool> RequestAccountDeletionAsync(int userId);
    }
}


