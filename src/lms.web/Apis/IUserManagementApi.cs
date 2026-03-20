using lms.shared.common.DTOs.usermanagement;
using Refit;


namespace lms.web.Apis
{
    public record RegisterRequest(RegisterUserDto RegisterUser);
    public interface IUserManagementApi
    {
        [Post("/api/v1/user")]
        Task<HttpResponseMessage> RegisterAsync(RegisterRequest request);

        [Get("/api/v1/user")]
        Task<HttpResponseMessage> GetUserByEmailAsync([AliasAs("userEmail")] string emailId);

        //[Get("/api/v1/user/{emailId}")]
        //Task<IList<string>> GetRolesByEmailAsync(string emailId);

        //[Put("/api/v1/user/{emailId}/roles")]
        //Task AssginRoleAsync(string id, [Body] AssigneRoleRequest request);

        //[Delete("/api/v1/user/{emailId}/roles")]
        //Task RemoveRoleAsync(string id, [Body] RemoveRoleRequest request);
    }

    public class AssigneRoleRequest { public required IList<Roles> UserRoles { get; set; } }
    public class RemoveRoleRequest { public string Role { get; set; } = string.Empty; }
    
}
