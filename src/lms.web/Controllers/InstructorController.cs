using lms.shared.common.DTOs.usermanagement;
using lms.web.Models;
using lms.web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace lms.web.Controllers
{
    public class InstructorController : Controller
    {
        private readonly string[] Steps = new[] { "Experience", "Topics", "Commitment", "Goals" };
        private readonly UserManagementService _userService;
        private readonly ILogger<InstructorController> _logger;

        public InstructorController(UserManagementService userService, ILogger<InstructorController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ── Public marketing landing page ─────────────────────────────────────
        // Authenticated + already an instructor → redirect straight to Dashboard.
        // Authenticated + onboarding completed  → redirect straight to Dashboard.
        // Otherwise show the "Teach on LMS" marketing page.

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Already has the role in their cookie — go straight to dashboard
                if (User.IsInRole("Instructor"))
                    return RedirectToAction("Dashboard");

                // Check DB in case role was assigned in a previous session
                string email = GetCurrentUserEmail();
                var status = await _userService.GetOnboardingStatusAsync(email);
                if (status?.Status == "Completed")
                    return RedirectToAction("Dashboard");
            }

            return View();
        }

        // ── Onboarding wizard ─────────────────────────────────────────────────

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Onboarding()
        {
            string email = GetCurrentUserEmail();

            // Already completed → skip wizard entirely
            if (User.IsInRole("Instructor"))
                return RedirectToAction("Dashboard");

            // Load existing progress from DB so user resumes where they left off
            var saved = await _userService.GetOnboardingStatusAsync(email);

            if (saved?.Status == "Completed")
                return RedirectToAction("Dashboard");

            OnboardingViewModel model = saved != null ? MapToViewModel(saved) : new OnboardingViewModel();
            // Always start at the next incomplete step; minimum step 1
            model.CurrentStep = saved != null ? Math.Max(1, saved.LastStep) : 1;
            model.Steps = Steps;

            return View("Onboarding", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onboarding(OnboardingViewModel model)
        {
            string email = GetCurrentUserEmail();
            bool isComplete = model.CurrentStep > 4;

            if (!isComplete && !ValidateCurrentStep(model))
            {
                model.Steps = Steps;
                return View("Onboarding", model);
            }

            // Clamp step to 4 for the final save
            if (isComplete) model.CurrentStep = 4;

            // Persist to DB — role is assigned inside the service when isComplete=true
            var saved = await _userService.SaveOnboardingStepAsync(email, model, isComplete);
            if (!saved)
            {
                _logger.LogError("Failed to save onboarding step {Step} for {Email}",
                    model.CurrentStep, email);
                ModelState.AddModelError("", "Could not save your progress. Please try again.");
                model.Steps = Steps;
                return View("Onboarding", model);
            }

            if (isComplete)
            {
                // Re-issue the auth cookie to include the new Instructor role claim
                // so the nav switches immediately without forcing a logout/login.
                await RefreshRoleClaimsAsync(email);

                TempData["DashboardWelcome"] = "true";
                return RedirectToAction("Dashboard");
            }

            model.Steps = Steps;
            model.CurrentStep++;
            return View("Onboarding", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrevStep(OnboardingViewModel model)
        {
            // Save current data before going back so nothing is lost
            string email = GetCurrentUserEmail();
            await _userService.SaveOnboardingStepAsync(email, model, isComplete: false);

            model.CurrentStep = Math.Max(1, model.CurrentStep - 1);
            model.Steps = Steps;
            return View("Onboarding", model);
        }

        // ── Dashboard ────────────────────────────────────────────────────────

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            InstructorProfileViewModel model = await BuildProfileModelAsync();
            ViewBag.ActiveSection = "overview";
            ViewBag.ShowWelcomeBanner = TempData["DashboardWelcome"] != null;
            return View("Dashboard", model);
        }

        // ── Profile ──────────────────────────────────────────────────────────

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            InstructorProfileViewModel model = await BuildProfileModelAsync();
            ViewBag.ActiveSection = "profile";
            return View("Dashboard", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(InstructorProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActiveSection = "profile";
                return View("Dashboard", model);
            }

            string email = GetCurrentUserEmail();
            try
            {
                await _userService.UpdateProfileAsync(email, model);
                model.IsSuccess = true;
                TempData["ProfileSuccess"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile for {Email}", email);
                model.ErrorMessage = "Failed to update profile. Please try again.";
            }

            ViewBag.ActiveSection = "profile";
            return View("Dashboard", model);
        }

        // ── Courses ──────────────────────────────────────────────────────────

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            InstructorProfileViewModel model = await BuildProfileModelAsync();
            ViewBag.ActiveSection = "courses";
            return View("Dashboard", model);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private string GetCurrentUserEmail()
            => User.FindFirst(ClaimTypes.Email)?.Value ?? "";

        private async Task<InstructorProfileViewModel> BuildProfileModelAsync()
        {
            string email = GetCurrentUserEmail();
            try
            {
                UserDto? user = await _userService.GetUserByEmailAsync(email);
                return new InstructorProfileViewModel
                {
                    FirstName = user?.FirstName ?? "",
                    LastName = user?.LastName ?? "",
                    Headline = user?.Headline,
                    Bio = user?.Bio,
                    Website = user?.Website,
                    LinkedInUrl = user?.LinkedInUrl,
                    TwitterHandle = user?.TwitterHandle,
                    Language = user?.Language ?? "English"
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load user profile for {Email}", email);
                return new InstructorProfileViewModel();
            }
        }

        /// <summary>
        /// Re-signs the auth cookie with fresh role claims after onboarding completes.
        /// This means "Teach on LMS" swaps to "Dashboard" immediately in the nav
        /// without the user needing to log out and back in.
        /// </summary>
        private async Task RefreshRoleClaimsAsync(string email)
        {
            try
            {
                UserRolesDto rolesDto = await _userService.GetUserRoles(email);
                IList<string> roles = rolesDto.UserRoles ?? new List<string>();

                var claims = new List<System.Security.Claims.Claim>
                {
                    new(ClaimTypes.NameIdentifier, email),
                    new(ClaimTypes.Email,          email)
                };
                foreach (string role in roles)
                    claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, role));

                var identity = new System.Security.Claims.ClaimsIdentity(
                    claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new System.Security.Claims.ClaimsPrincipal(identity);
                var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                    principal, properties);

                _logger.LogInformation("Cookie refreshed for {Email} with roles: {Roles}",
                    email, string.Join(", ", roles));
            }
            catch (Exception ex)
            {
                // Non-fatal — user will see updated nav on next login
                _logger.LogWarning(ex, "Could not refresh role claims for {Email}", email);
            }
        }

        private static OnboardingViewModel MapToViewModel(OnboardingStatusDto s) => new()
        {
            YearsExperience = s.YearsExperience,
            PriorCourses = s.PriorCourses ?? "",
            Specialties = s.Specialties ?? "",
            HasRecordingEquipment = s.HasRecordingEquipment,
            CoursePlan = s.CoursePlan ?? "",
            Headline = s.Headline ?? "",
            Bio = s.Bio ?? "",
            Language = s.Language ?? "English",
            PayoutMethod = s.PayoutMethod ?? ""
        };

        private bool ValidateCurrentStep(OnboardingViewModel model) =>
            model.CurrentStep switch
            {
                1 => model.YearsExperience >= 0,
                2 => !string.IsNullOrEmpty(model.Specialties),
                3 => !string.IsNullOrEmpty(model.CoursePlan),
                4 => true,
                _ => false
            };
    }
}
