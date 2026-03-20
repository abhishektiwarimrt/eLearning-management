using lms.shared.data.entities.instructormanagement;

namespace lms.shared.data.repositories.instructormanagement
{
    public interface IOnboardingRepository
    {
        /// <summary>Returns null if the user has never started onboarding.</summary>
        Task<InstructorOnboardingStatus?> GetByUserIdAsync(int userId);

        /// <summary>Returns null if the user has never started onboarding.</summary>
        Task<InstructorOnboardingStatus?> GetByEmailAsync(string email);

        /// <summary>Insert or update — safe to call on every step save.</summary>
        Task UpsertAsync(InstructorOnboardingStatus status);
    }
}
