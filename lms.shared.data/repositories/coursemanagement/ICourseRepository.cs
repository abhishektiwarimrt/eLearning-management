using lms.shared.data.entities.coursemanagement;

namespace lms.shared.data.repositories.coursemanagement
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(Guid id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> GetByCreatorIdAsync(int creatorId);
        Task<Course> AddAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task DeleteAsync(Course course);
    }
}
