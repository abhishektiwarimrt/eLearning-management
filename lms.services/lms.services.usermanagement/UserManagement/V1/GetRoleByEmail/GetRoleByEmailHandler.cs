namespace lms.services.usermanagement.UserManagement.V1.GetRoleByEmail
{
    public record GetUserRoleByEmailQuery(string UserEmail)
       : IQuery<GetUserRoleByEmailResult>;
    public record GetUserRoleByEmailResult(IList<string> UserRoles);
    public class GetRoleByEmailHandler(IRoleService roleService, ILogger<GetRoleByEmailHandler> logger)
        : IQueryHandler<GetUserRoleByEmailQuery, GetUserRoleByEmailResult>
    {
        public async Task<GetUserRoleByEmailResult> Handle(GetUserRoleByEmailQuery query, CancellationToken cancellationToken)
        {
            var roles = await roleService.GetUserRolesByEmailAsync(query.UserEmail);

            if (roles == null)
            {
                logger.LogError($"Roles not found for user:{query.UserEmail}");
                throw new NotFoundException("Roles Not Found");
            }

            return new GetUserRoleByEmailResult(roles);
        }
    }
}
