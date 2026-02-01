using lms.shared.data.dbcontexts;
using lms.shared.data.entities.usermanagement;
using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.repositories.usermanagement
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserRepository(
            UserDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult?> CreateAsync(User user)
        {
            user.UserName = user.Email;
            return await _userManager.CreateAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByNameAsync(email);
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
