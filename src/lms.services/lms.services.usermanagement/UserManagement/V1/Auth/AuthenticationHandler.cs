using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace lms.services.usermanagement.UserManagement.V1.Auth
{
    public record UserAuthResponse(bool Success,
    string? Token = null,
    string? Error = null,
    string? UserId = null,
    string? Email = null,
    string[]? Roles = null,
    DateTime? Expires = null);
    public record UserAuthCommand(LoginUserDto UserLogin) : IRequest<UserAuthResponse>;
#pragma warning disable CS9113
    public class AuthenticationHandler(IUserService userService, IRoleService roleService, IUnitOfWork<UserDbContext> _unitOfWork, ILogger<AuthenticationHandler> _logger, IConfiguration _configuration)
#pragma warning restore CS9113
        : IRequestHandler<UserAuthCommand, UserAuthResponse>
    {

        public async Task<UserAuthResponse> Handle(UserAuthCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for {Email}", request.UserLogin.Email);

            try
            {
                var isAuthenticated = await userService.ValidateUserCredentialsAsync(request.UserLogin.Email, request.UserLogin.Password);
                if (!isAuthenticated) return new UserAuthResponse(false, Error: "Invalid credentials");

                var user = await userService.GetUserByEmailAsync(request.UserLogin.Email);
                if (user == null)
                {
                    _logger.LogError("User not found after validation for: {Email}", request.UserLogin.Email);
                    return new UserAuthResponse(false, Error: "User not found");
                }

                var roles = await roleService.GetUserRolesByEmailAsync(request.UserLogin.Email);
                _logger.LogInformation("Login successful for user: {Email} with roles: [{Roles}]",
                    request.UserLogin.Email, string.Join(", ", roles));

                var (token, expires) = GenerateJwtToken(user, roles);

                return new UserAuthResponse(
                    true,
                    Token: token,
                    UserId: user.Id.ToString(),
                    Email: user.Email!,
                    Roles: roles.ToArray(),
                    Expires: expires
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", request.UserLogin.Email);
                return new(false, Error: "Internal server error");
            }
        }

        private (string token, DateTime expires) GenerateJwtToken(UserDto user, IList<string> roles)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryHours = jwtSection.GetValue<int>("ExpiryHours", 8);
            var expires = DateTime.UtcNow.AddHours(expiryHours);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}
