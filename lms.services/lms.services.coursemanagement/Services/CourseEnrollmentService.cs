

namespace lms.services.coursemanagement.Services
{
    public class CourseEnrollmentService : ICourseEnrollmentService
    {
        public Task<EnrollmentDto> CreateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEnrollmentAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<EnrollmentDto> GetEnrollmentByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseIdAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
