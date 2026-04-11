
namespace lms.services.usermanagement.UserManagement.V1.Auth
{
    public record UserAuthResponse(bool Success,
    string? Token = null,
    string? Error = null,
    string? UserId = null,
    string? Email = null,
    string[]? Roles = null);
    public record UserAuthCommand(LoginUserDto UserLogin) : IRequest<UserAuthResponse>;
#pragma warning disable CS9113
    public class AuthenticationHandler (IUserService userService, IRoleService roleService, IUnitOfWork<UserDbContext> _unitOfWork, ILogger<AuthenticationHandler> _logger)
#pragma warning restore CS9113
        : IRequestHandler<UserAuthCommand, UserAuthResponse>
    {

        public async Task<UserAuthResponse> Handle(UserAuthCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for {Email}", request.UserLogin.Email);

            try
            {
                // TODO: Validate credentials against DB
                var isAuthentiated = await userService.ValidateUserCredentialsAsync(request.UserLogin.Email, request.UserLogin.Password);
                if (!isAuthentiated) return new UserAuthResponse(false, Error: "Invalid credentials");

                // Step 2: Get user details
                var user = await userService.GetUserByEmailAsync(request.UserLogin.Email);
                if (user == null)
                {
                    _logger.LogError("User not found after validation for: {Email}", request.UserLogin.Email);
                    return new UserAuthResponse(false, Error: "User not found");
                }
                var roles = await roleService.GetUserRolesByEmailAsync(request.UserLogin.Email); // Direct call
                _logger.LogInformation("Login successful for user: {Email} with roles: [{Roles}]",
                request.UserLogin.Email, string.Join(", ", roles));

                // TODO: Generate JWT
                // var token = _jwtService.GenerateToken(user);
                //var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // Placeholder JWT

                return new UserAuthResponse(
                    true,
                    Token: null,  // No JWT yet
                    UserId: user.Id.ToString(),
                    Email: user.Email!,
                    Roles: roles.ToArray()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.UserLogin.Email);
                return new(false, Error: "Internal server error");
            }
        }
    }
}