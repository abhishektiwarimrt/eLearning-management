using lms.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace lms.web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Show different content based on auth status
            //if (User.Identity?.IsAuthenticated == true)
            //{
            //    ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            //    ViewBag.UserRoles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
            //    return View("Dashboard"); // Authenticated dashboard
            //}

            //return View();

            var model = new HomeViewModel
            {
                FeaturedCourses = GetFeaturedCourses(),
                PopularCategories = GetCategories(),
                Stats = new HomeStats { TotalCourses = 15000, TotalStudents = 5000000, Instructors = 25000 }
            };
            return View(model);
        }

        [Authorize] // Only logged-in users
        public IActionResult Dashboard()
        {
            return View();
        }

        private List<FeaturedCourse> GetFeaturedCourses() => new()
        {
            new FeaturedCourse { Id = 1, Title = "Complete .NET 8 Microservices", Instructor = "John Smith", Rating = 4.8, Students = 12500, Price = 129, OriginalPrice = 199 },
            new FeaturedCourse { Id = 2, Title = "Vue.js Masterclass 2026", Instructor = "Sarah Johnson", Rating = 4.9, Students = 8900, Price = 89, OriginalPrice = 149 },
            new FeaturedCourse { Id = 3, Title = "Docker & Kubernetes", Instructor = "Mike Chen", Rating = 4.7, Students = 21000, Price = 99, OriginalPrice = 179 }
        };

        private List<Category> GetCategories() => new()
        {
            new Category { Name = "Development", Icon = "code.svg", CoursesCount = 4500 },
            new Category { Name = "Design", Icon = "brush.svg", CoursesCount = 2300 },
            new Category { Name = "Business", Icon = "chart-bar.svg", CoursesCount = 1800 },
            new Category { Name = "Marketing", Icon = "megaphone.svg", CoursesCount = 1200 },
            new Category { Name = "IT & Software", Icon = "server.svg", CoursesCount = 3200 },
            new Category { Name = "Personal Development", Icon = "sparkles.svg", CoursesCount = 900 }
        };

    }  

    public class FeaturedCourse
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Instructor { get; set; } = "";
        public double Rating { get; set; }
        public int Students { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
    }

    public class Category
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";  // SVG filename: "play.svg"
        public int CoursesCount { get; set; }
    }


    public class HomeStats
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int Instructors { get; set; }
    }

}
