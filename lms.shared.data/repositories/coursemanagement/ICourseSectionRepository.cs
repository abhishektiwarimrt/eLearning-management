using lms.shared.data.entities.coursemanagement;

namespace lms.shared.data.repositories.coursemanagement
{
    public interface ICourseSectionRepository
    {
        Task<CourseSection> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseSection>> GetByCourseIdAsync(Guid courseId);
        Task<int> GetNextOrderForCourseAsync(Guid courseId);
        Task<IList<CourseSection>> AddAsync(IList<CourseSection> sections);
        Task<CourseSection> UpdateAsync(CourseSection section);
        Task DeleteAsync(CourseSection section);
    }
}
