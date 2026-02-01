

namespace lms.services.usermanagement.Services
{
    public class RoleService
        (IUserRepository userRepository
        , IRoleRepository repository
        , ILogger<RoleService> logger)
        : IRoleService
    {
        public async Task<bool?> AddToRoleAsync(string userEmail, string roleName)
        {

            var user = await GetUserAndValidateExistence(userEmail);
            var result = await repository.AddToRoleAsync(user, roleName);

            if (result != null && !result.Succeeded)
            {
                var errorMessages = string.Join(Environment.NewLine, result.Errors.Select(err => $"•{err.Code}: {err.Description}"));
                logger.LogError(errorMessages);
            }

            return result?.Succeeded;
        }

        public async Task<IList<string>> GetUserRolesByEmailAsync(string email)
        {
            var user = await GetUserAndValidateExistence(email);
            return await repository.GetUserRolesAsync(user);
        }

        private async Task<User> GetUserAndValidateExistence(string userEmail)
        {
            var user = await userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                var message = $"User:{userEmail} Not Found!";
                logger.LogError(message);
                throw new NotFoundException(message);
            }

            return user;
        }
    }
}
