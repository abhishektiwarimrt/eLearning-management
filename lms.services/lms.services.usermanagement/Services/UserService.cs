
namespace lms.services.usermanagement.Services
{
    public class UserService(IUserRepository _userRepository, ILogger<UserService> logger) : IUserService
    {
        public async Task<bool?> CreateUserAsync(RegisterUserDto registerUserDto)
        {

            var user = new User
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                PasswordHash = registerUserDto.Password,
                // Set other properties as needed
            };
            var createUserresult = await _userRepository.CreateAsync(user);

            if (createUserresult != null && !createUserresult.Succeeded)
            {
                var errorMessages = string.Join(Environment.NewLine, createUserresult.Errors.Select(err => $"•{err.Code}: {err.Description}"));
                logger.LogError(errorMessages);
            }

            return createUserresult?.Succeeded;

        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = (await _userRepository.GetUserByEmailAsync(email)) ?? throw new NotFoundException(email);

            return user.Adapt<UserDto>();
        }

        //public Task<bool> AddUserToRoleAsync(int userId, string roleName)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> CreateOrUpdateUserFromExternalLoginAsync(ExternalLoginInfo externalLoginInfo)
        //{
        //    throw new NotImplementedException();
        //}



        //public Task DeleteUserAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDataExportDto> ExportUserDataAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<DateTime?> GetLastLoginDateAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> GetUserByEmailAsync(string email)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> GetUserByIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> GetUserCourseCountAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserProfileDto> GetUserProfileAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> HasPermissionAsync(int userId, string permission)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> IsEmailVerifiedAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> LinkExternalLoginAsync(int userId, ExternalLoginInfo externalLoginInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> LockUserAccountAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> RemoveUserFromRoleAsync(int userId, string roleName)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> RequestAccountDeletionAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ResetPasswordAsync(string email)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> SendVerificationEmailAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> UnlockUserAccountAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateLastLoginDateAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<NotificationPreferencesDto> UpdateNotificationPreferencesAsync(int userId, UpdateNotificationPreferencesDto preferencesDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ValidateUserCredentialsAsync(string email, string password)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> VerifyEmailAsync(int userId, string token)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
