using lms.shared.data.entities.usermanagement;
using Microsoft.AspNetCore.Identity;

namespace lms.shared.data.repositories.usermanagement
{
    public interface IRoleRepository
    {
        Task<IdentityResult> AddToRoleAsync(User user, string roleName);
        Task<IList<string>> GetUserRolesAsync(User user);
    }
}
