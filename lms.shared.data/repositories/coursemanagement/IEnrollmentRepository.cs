using lms.shared.data.entities.coursemanagement;

namespace lms.shared.data.repositories.coursemanagement
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId);
        Task<Enrollment> AddAsync(Enrollment enrollment);
        Task DeleteAsync(Enrollment enrollment);
    }
}
