using lms.shared.data.entities.coursemanagement.Content;

namespace lms.services.coursemanagement.Services
{
    public interface ICourseModuleService
    {
        Task<CourseModuleDto> GetModuleByIdAsync(Guid id);
        Task<IEnumerable<CourseModuleDto>> GetModulesByCourseIdAsync(Guid courseId);
        Task<IList<CourseModuleDto>> CreateModuleAsync(Guid CourseId, Guid CourseSectionId, IList<CourseModuleDto> CourseModules);
        Task<CourseModuleDto> UpdateModuleAsync(CourseModuleDto moduleDto);
        Task DeleteModuleAsync(Guid id);
    }

}
