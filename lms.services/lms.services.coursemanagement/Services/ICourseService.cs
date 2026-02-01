
namespace lms.services.coursemanagement.Services
{
    public interface ICourseService
    {
        Task<CourseDto> GetCourseByIdAsync(Guid id);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> CreateCourseAsync(CourseDto courseDto);
        Task<CourseDto> UpdateCourseAsync(CourseDto courseDto);
        Task DeleteCourseAsync(Guid id);
    }
}

