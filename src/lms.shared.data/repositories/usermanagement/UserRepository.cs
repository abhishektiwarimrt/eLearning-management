using lms.shared.data.dbcontexts;
using lms.shared.data.entities.usermanagement;
using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.repositories.usermanagement
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(
            UserDbContext context,
            UserManager<User> userManager,
            IRoleRepository roleRepository)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IdentityResult?> CreateAsync(User user)
        {
            user.UserName = user.Email;
            return await _userManager.CreateAsync(user, user.PasswordHash);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByNameAsync(email);
        }

        /// <summary>
        /// Persists changes to an existing User entity.
        /// Uses Identity's UpdateAsync which handles concurrency stamps automatically.
        /// </summary>
        public async Task<IdentityResult?> UpdateAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            User? user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return false;
            }

            // Use Identity's built-in password verifier (handles hashing)
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        //public Task<bool> AddToRoleAsync(int userId, string roleName)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        //{
        //    throw new NotImplementedException();
        //}



        //public Task<UserDto> CreateOrUpdateFromExternalLoginAsync(ExternalLoginInfo externalLoginInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task DeleteAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDataExportDto> ExportUserDataAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> GetByEmailAsync(string email)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> GetByIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<UserDto>> GetByRoleAsync(string roleName, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> GetCourseCountAsync(int userId)
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

        //public Task<UserProfileDto> GetProfileAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<string>> GetRolesAsync(int userId)
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

        //public Task<bool> LockAccountAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> MarkForDeletionAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> RemoveFromRoleAsync(int userId, string roleName)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ResetPasswordAsync(string email)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, int page, int pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> SetEmailVerifiedAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> UnlockAccountAsync(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto)
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

        //public Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ValidateCredentialsAsync(string email, string password)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
