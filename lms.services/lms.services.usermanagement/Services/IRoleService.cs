namespace lms.services.usermanagement.Services
{
    public interface IRoleService
    {
        Task<bool?> AddToRoleAsync(string userEmail, string roleName);
        Task<IList<string>> GetUserRolesByEmailAsync(string email);
        //Task<bool> DeleteRoleAsync(string roleId);
        //Task<IEnumerable<Role>> GetAllRolesAsync();
        //Task<Role> GetRoleByIdAsync(string roleId);
        //Task<bool> UpdateRoleAsync(Role role);
        //Task<bool> AssignRoleToUserAsync(string userId, string roleId);
        //Task<bool> RemoveRoleFromUserAsync(string userId, string roleId);
        //Task<IEnumerable<Role>> GetUserRolesAsync(string userId);
        //Task<bool> UserHasRoleAsync(string userId, string roleName);
        //Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName, int page, int pageSize);
        //Task<bool> HasPermissionAsync(int userId, string permission);
    }

}
