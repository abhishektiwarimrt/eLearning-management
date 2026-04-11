using lms.buildingblocks.RequestResponse;
using lms.shared.common.DTOs.usermanagement;
using lms.web.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lms.web.Services
{
    public class UserManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<UserManagementService> _logger;

        // Named HttpClient registered in Program.cs with JwtForwardingHandler
        public const string HttpClientName = "UserManagement";

        public UserManagementService(IHttpClientFactory httpClientFactory, ILogger<UserManagementService> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(HttpClientName);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        // ── Auth ─────────────────────────────────────────────────────────────

        public async Task<LoginResponseDto> LoginAsync(LoginUserDto loginDto)
        {
            try
            {
                StringContent content = ToJsonContent(new { userLogin = loginDto });
                HttpResponseMessage response = await _httpClient.PostAsync("api/v1/Auth/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Login failed: {Status}", response.StatusCode);
                    return new LoginResponseDto(false, Error: "Invalid credentials");
                }

                string json = await response.Content.ReadAsStringAsync();
                ApiResponse<LoginResponseDto>? result = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(json, _jsonOptions);
                return result?.Data ?? new LoginResponseDto(false, Error: "Invalid response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login service error");
                return new LoginResponseDto(false, Error: "Service unavailable");
            }
        }

        // ── User ─────────────────────────────────────────────────────────────

        public async Task<HttpResponseMessage> RegisterAsync(RegisterUserDto user)
        {
            string json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/v1/user", content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"api/v1/user?userEmail={Uri.EscapeDataString(email)}");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            ApiResponse<GetUserByEmailResponse>? apiResponse = JsonSerializer.Deserialize<ApiResponse<GetUserByEmailResponse>>(json, _jsonOptions);
            return apiResponse?.Data?.User;
        }

        public async Task UpdateProfileAsync(string email, InstructorProfileViewModel profile)
        {
            UpdateUserProfileRequest payload = new UpdateUserProfileRequest(
                email, profile.FirstName, profile.LastName,
                profile.Headline, profile.Bio, profile.Website,
                profile.LinkedInUrl, profile.TwitterHandle, profile.Language);

            HttpResponseMessage response = await _httpClient.PutAsync(
                $"api/v1/user/{Uri.EscapeDataString(email)}/profile",
                ToJsonContent(payload));
            response.EnsureSuccessStatusCode();
        }

        // ── Roles ─────────────────────────────────────────────────────────────

        public async Task<UserRolesDto> GetUserRoles(string emailId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"api/v1/User/{Uri.EscapeDataString(emailId)}/Roles");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            ApiResponse<UserRolesDto>? apiResponse = JsonSerializer.Deserialize<ApiResponse<UserRolesDto>>(json, _jsonOptions);
            return apiResponse?.Data ?? new UserRolesDto();
        }

        public async Task AssignRoleAsync(string email, string roleName)
        {
            int roleIndex = roleName switch
            {
                "Admin" => 0,
                "Instructor" => 1,
                "Student" => 2,
                _ => throw new ArgumentException($"Unknown role: {roleName}")
            };
            HttpResponseMessage response = await _httpClient.PostAsync(
                $"api/v1/User/{Uri.EscapeDataString(email)}/Roles",
                ToJsonContent(new { UserRoles = new[] { roleIndex } }));
            response.EnsureSuccessStatusCode();
        }

        // ── Onboarding ────────────────────────────────────────────────────────

        public async Task<OnboardingStatusDto?> GetOnboardingStatusAsync(string email)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(
                    $"api/v1/user/{Uri.EscapeDataString(email)}/onboarding");

                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                ApiResponse<OnboardingStatusDto>? apiResponse = JsonSerializer.Deserialize<ApiResponse<OnboardingStatusDto>>(json, _jsonOptions);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not fetch onboarding status for {Email}", email);
                return null;
            }
        }

        public async Task<bool> SaveOnboardingStepAsync(string email, OnboardingViewModel model, bool isComplete)
        {
            var payload = new
            {
                UserEmail = email,
                LastStep = model.CurrentStep,
                IsComplete = isComplete,
                YearsExperience = model.YearsExperience,
                PriorCourses = model.PriorCourses,
                Specialties = model.Specialties,
                HasRecordingEquipment = model.HasRecordingEquipment,
                CoursePlan = model.CoursePlan,
                Headline = model.Headline,
                Bio = model.Bio,
                Language = model.Language,
                PayoutMethod = model.PayoutMethod,
            };

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"api/v1/user/{Uri.EscapeDataString(email)}/onboarding",
                ToJsonContent(payload));

            return response.IsSuccessStatusCode;
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private StringContent ToJsonContent(object obj)
            => new(JsonSerializer.Serialize(obj, _jsonOptions), Encoding.UTF8, "application/json");

        private record GetUserByEmailResponse(UserDto User);
    }
}

public record LoginResponseDto(bool Success, string? Token = null, string? Error = null, string[]? Roles = null);

public record UpdateUserProfileRequest(
    string Email, string FirstName, string LastName,
    string? Headline, string? Bio, string? Website,
    string? LinkedInUrl, string? TwitterHandle, string? Language);

/// <summary>Mirrors GetOnboardingStatusResponse from the service.</summary>
public class OnboardingStatusDto
{
    public bool Exists { get; set; }
    public string Status { get; set; } = "";
    public int LastStep { get; set; }
    public int YearsExperience { get; set; }
    public string? PriorCourses { get; set; }
    public string? Specialties { get; set; }
    public bool HasRecordingEquipment { get; set; }
    public string? CoursePlan { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public string? Language { get; set; }
    public string? PayoutMethod { get; set; }
}
