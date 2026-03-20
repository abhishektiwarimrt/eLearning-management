using lms.web.Controllers;

namespace lms.web.Models
{
    public class HomeViewModel
    {
        public List<FeaturedCourse> FeaturedCourses { get; set; } = new();
        public List<Category> PopularCategories { get; set; } = new();
        public HomeStats Stats { get; set; } = new();
    }
}
