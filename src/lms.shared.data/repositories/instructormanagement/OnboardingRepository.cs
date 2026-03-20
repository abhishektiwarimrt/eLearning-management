using lms.shared.data.dbcontexts;
using lms.shared.data.entities.instructormanagement;
using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.repositories.instructormanagement
{
    public class OnboardingRepository(UserDbContext db) : IOnboardingRepository
    {
        public async Task<InstructorOnboardingStatus?> GetByUserIdAsync(int userId)
            => await db.InstructorOnboardingStatuses
                       .FirstOrDefaultAsync(o => o.UserId == userId);

        public async Task<InstructorOnboardingStatus?> GetByEmailAsync(string email)
            => await db.InstructorOnboardingStatuses
                       .Include(o => o.User)
                       .FirstOrDefaultAsync(o => o.User.Email == email);

        public async Task UpsertAsync(InstructorOnboardingStatus status)
        {
            InstructorOnboardingStatus? existing = await db.InstructorOnboardingStatuses
                                   .FirstOrDefaultAsync(o => o.UserId == status.UserId);

            if (existing == null)
            {
                status.CreatedAt = DateTime.UtcNow;
                status.UpdatedAt = DateTime.UtcNow;
                await db.InstructorOnboardingStatuses.AddAsync(status);
            }
            else
            {
                // Merge in all fields — never overwrite Status back to InProgress
                existing.LastStep = status.LastStep;
                existing.YearsExperience = status.YearsExperience;
                existing.PriorCourses = status.PriorCourses;
                existing.Specialties = status.Specialties;
                existing.HasRecordingEquipment = status.HasRecordingEquipment;
                existing.CoursePlan = status.CoursePlan;
                existing.Headline = status.Headline;
                existing.Bio = status.Bio;
                existing.Language = status.Language;
                existing.PayoutMethod = status.PayoutMethod;
                existing.UpdatedAt = DateTime.UtcNow;

                // Only allow forward transitions
                if (status.Status == OnboardingStatusValue.Completed
                    && existing.Status != OnboardingStatusValue.Completed)
                {
                    existing.Status = OnboardingStatusValue.Completed;
                    existing.CompletedAt = DateTime.UtcNow;
                }

                db.InstructorOnboardingStatuses.Update(existing);
            }

            await db.SaveChangesAsync();
        }
    }
}
