using lms.shared.data.entities.usermanagement;
using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.repositories.usermanagement
{
    public class RoleRepository(UserManager<User> userManager) : IRoleRepository
    {
        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            return await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await userManager.GetRolesAsync(user);
        }
    }
}
