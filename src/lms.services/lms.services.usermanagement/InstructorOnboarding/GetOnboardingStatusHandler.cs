using lms.shared.data.entities.instructormanagement;
using lms.shared.data.repositories.instructormanagement;

namespace lms.services.usermanagement.InstructorOnboarding
{
    // ── Get query ─────────────────────────────────────────────────────────────

    public record GetOnboardingStatusQuery(string Email)
        : IQuery<GetOnboardingStatusResponse>;

    public class GetOnboardingStatusHandler(IOnboardingRepository onboardingRepo, IUserRepository userRepo)
        : IQueryHandler<GetOnboardingStatusQuery, GetOnboardingStatusResponse>
    {
        public async Task<GetOnboardingStatusResponse> Handle(
            GetOnboardingStatusQuery query, CancellationToken ct)
        {
            User user = await userRepo.GetUserByEmailAsync(query.Email)
                       ?? throw new NotFoundException(query.Email);

            InstructorOnboardingStatus? record = await onboardingRepo.GetByUserIdAsync(user.Id);

            if (record == null)
                return new GetOnboardingStatusResponse(
                    Exists: false,
                    Status: OnboardingStatusValue.InProgress,
                    LastStep: 0,
                    YearsExperience: 0,
                    PriorCourses: null, Specialties: null,
                    HasRecordingEquipment: false,
                    CoursePlan: null, Headline: null, Bio: null,
                    Language: null, PayoutMethod: null);

            return new GetOnboardingStatusResponse(
                Exists: true,
                Status: record.Status,
                LastStep: record.LastStep,
                YearsExperience: record.YearsExperience,
                PriorCourses: record.PriorCourses,
                Specialties: record.Specialties,
                HasRecordingEquipment: record.HasRecordingEquipment,
                CoursePlan: record.CoursePlan,
                Headline: record.Headline,
                Bio: record.Bio,
                Language: record.Language,
                PayoutMethod: record.PayoutMethod);
        }
    }
}
