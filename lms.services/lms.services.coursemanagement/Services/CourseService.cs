
namespace lms.services.coursemanagement.Services
{
    public class CourseService(ICourseRepository courseRepository) : ICourseService
    {

        public async Task<CourseDto> CreateCourseAsync(CourseDto courseDto)
        {
            var course = courseDto.Adapt<Course>();
            var AddedCourse = await courseRepository.AddAsync(course);
            var result = AddedCourse.Adapt<CourseDto>();

            return result;
        }

        public Task DeleteCourseAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CourseDto> GetCourseByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CourseDto> UpdateCourseAsync(CourseDto courseDto)
        {
            throw new NotImplementedException();
        }
    }
}
