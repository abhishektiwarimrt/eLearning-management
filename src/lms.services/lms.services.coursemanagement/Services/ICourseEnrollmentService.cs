using lms.shared.common.DTOs.coursemanagement;

namespace lms.services.coursemanagement.Services
{
    public interface ICourseEnrollmentService
    {
        Task<EnrollmentDto> GetEnrollmentByIdAsync(Guid id);
        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByUserIdAsync(int userId);
        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseIdAsync(Guid courseId);
        Task<EnrollmentDto> CreateEnrollmentAsync(EnrollmentDto enrollmentDto);
        Task DeleteEnrollmentAsync(Guid id);
    }
}


