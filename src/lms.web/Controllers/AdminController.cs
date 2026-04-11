using lms.web.Apis;
using lms.web.Models;
using lms.web.Services;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace lms.web.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManagementService _userService;

        public AdminController(UserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> UserRoles(string emailId)
        {
            try
            {
                emailId = "aa@aa.com";
                UserRolesViewModel? model = null;
                var user = await _userService.GetUserByEmailAsync(emailId);
                var currentUserRoles = await _userService.GetUserRoles(emailId);
                var knownRoles = new[] { "Admin", "Instructor", "Student" }; // Constrained roles [cite:6][cite:7]
                if (user != null)
                {
                    model = new UserRolesViewModel
                    {
                        UserId = emailId,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        DateOfBirth = DateTime.Now,
                        CurrentRoles = currentUserRoles.UserRoles ?? new List<string>(),
                        AvailableRoles = knownRoles.Except(currentUserRoles.UserRoles ?? new List<string>()).ToList()
                    };
                }

                return View(model);
            }
            catch(Exception)
            {
                throw;
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> AssignRole(string userId, string role)
        //{
        //    await _userApi.AddRoleAsync(userId, new AddRoleRequest { Role = role });
        //    TempData["Success"] = $"Role '{role}' assigned to user.";
        //    return RedirectToAction(nameof(UserRoles), new { id = userId });
        //}

        //[HttpPost]
        //public async Task<IActionResult> RemoveRole(string userId, string role)
        //{
        //    await _userApi.RemoveRoleAsync(userId, new RemoveRoleRequest { Role = role });
        //    TempData["Success"] = $"Role '{role}' removed from user.";
        //    return RedirectToAction(nameof(UserRoles), new { id = userId });
        //}
    }
}

