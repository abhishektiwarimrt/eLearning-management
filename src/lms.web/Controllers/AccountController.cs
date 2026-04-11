using lms.shared.common.DTOs.usermanagement;
using lms.web.Models;
using lms.web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace lms.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManagementService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManagementService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                HttpResponseMessage response = await _userService.RegisterAsync(new RegisterUserDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,

                });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Account created! Please login.";
                    model.IsSuccess = true;
                }
                else
                {
                    model.ErrorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", model.ErrorMessage ?? "Registration failed");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Service unavailable. Try again later.");
            }

            return View(model);
        }

        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                LoginResponseDto response = await _userService.LoginAsync(new LoginUserDto
                {
                    Email = model.Email,
                    Password = model.Password
                });

                if (response.Success)
                {
                    // ── Build claims ──────────────────────────────────────────
                    List<Claim> claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, model.Email),
                        new(ClaimTypes.Email,          model.Email)
                    };

                    // Write each role as a separate Role claim so
                    // User.IsInRole("Instructor") works in _Layout.cshtml
                    // without any additional API calls per request.
                    if (response.Roles?.Length > 0)
                    {
                        foreach (var role in response.Roles)
                            claims.Add(new Claim(ClaimTypes.Role, role));

                        _logger.LogInformation("User {Email} logged in with roles: {Roles}",
                            model.Email, string.Join(", ", response.Roles));
                    }
                    else
                    {
                        // Roles not returned by login API — fetch separately as fallback
                        try
                        {
                            UserRolesDto rolesDto = await _userService.GetUserRoles(model.Email);
                            foreach (var role in rolesDto.UserRoles ?? new List<string>())
                                claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not fetch roles for {Email} during login", model.Email);
                        }
                    }

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                        new AuthenticationProperties { IsPersistent = model.RememberMe });

                    // Store the JWT so service calls can forward it as Bearer token
                    if (!string.IsNullOrEmpty(response.Token))
                        HttpContext.Session.SetString("jwt_token", response.Token);

                    return RedirectToAction("Index", "Home");
                }

                model.ErrorMessage = "Invalid email or password.";
                ModelState.AddModelError("", model.ErrorMessage);
            }
            catch
            {
                ModelState.AddModelError("", "Login service unavailable. Try again later.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("jwt_token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
