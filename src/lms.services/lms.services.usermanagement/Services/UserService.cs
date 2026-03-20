
using lms.services.usermanagement.UserManagement.V1.CreateUser;
using lms.services.usermanagement.UserManagement.V1.UpdateUserProfile;

namespace lms.services.usermanagement.Services
{
    public class UserService(IUserRepository _userRepository, IRoleRepository roleRepository, ILogger<UserService> logger) : IUserService
    {
        public async Task<bool?> CreateUserAsync(RegisterUserCommand request)
        {

            User user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = request.Password
                // Set other properties as needed
            };
            IdentityResult? createUserresult = await _userRepository.CreateAsync(user);

            if (createUserresult != null && !createUserresult.Succeeded)
            {
                string errorMessages = string.Join(Environment.NewLine, createUserresult.Errors.Select(err => $"•{err.Code}: {err.Description}"));
                logger.LogError(errorMessages);
            }

            return createUserresult?.Succeeded;

        }

        public Task<bool?> CreateUserAsync(RegisterUserDto registerUserDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            User user = (await _userRepository.GetUserByEmailAsync(email)) ?? throw new NotFoundException(email);

            return user.Adapt<UserDto>();
        }

        /// <summary>
        /// Updates mutable profile fields on the ASP.NET Identity User entity.
        /// Only updates non-null fields supplied in the request so partial updates
        /// are safe — existing values are preserved when a field is omitted.
        /// </summary>
        public async Task<bool> UpdateProfileAsync(string email, UpdateUserProfileRequest request)
        {
            User user = await _userRepository.GetUserByEmailAsync(email)
                       ?? throw new NotFoundException($"User '{email}' not found");

            // Map only the fields that the instructor can edit
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Headline = request.Headline ?? user.Headline;
            user.Bio = request.Bio ?? user.Bio;
            user.Website = request.Website ?? user.Website;
            user.LinkedInUrl = request.LinkedInUrl ?? user.LinkedInUrl;
            user.TwitterHandle = request.TwitterHandle ?? user.TwitterHandle;
            user.Language = request.Language ?? user.Language;

            IdentityResult? result = await _userRepository.UpdateAsync(user);

            if (result != null && !result.Succeeded)
            {
                string errors = string.Join(Environment.NewLine,
                    result.Errors.Select(e => $"• {e.Code}: {e.Description}"));
                logger.LogError("UpdateProfile failed for {Email}: {Errors}", email, errors);
                return false;
            }

            logger.LogInformation("Profile updated for {Email}", email);
            return true;
        }


        public Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            Task<bool> isValid = _userRepository.ValidateCredentialsAsync(email, password);
            return isValid;
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



        //public Task<bool> VerifyEmailAsync(int userId, string token)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
