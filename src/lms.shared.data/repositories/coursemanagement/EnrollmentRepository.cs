using lms.shared.data.entities.coursemanagement;

namespace lms.shared.data.repositories.coursemanagement
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        public Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
