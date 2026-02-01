namespace lms.services.usermanagement.UserManagement.V1.GetUserByEmail
{
    public record GetUserByEmailQuery(string Email)
       : IQuery<UserResult>;
    public record UserResult(UserDto User);
    public class GetUserByEmailHandler(IUserService userService)
        : IQueryHandler<GetUserByEmailQuery, UserResult>
    {
        public async Task<UserResult> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
        {
            var userDto = await userService.GetUserByEmailAsync(query.Email) ?? throw new NotFoundException(query.Email);

            return new UserResult(userDto);
        }
    }
}
