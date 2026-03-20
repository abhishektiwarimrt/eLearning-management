using lms.shared.data.entities.instructormanagement;
using lms.shared.data.repositories.instructormanagement;

namespace lms.services.usermanagement.InstructorOnboarding
{
    // ── Save command ─────────────────────────────────────────────────────────

    public record SaveOnboardingStepCommand(string Email, SaveOnboardingStepRequest Data)
        : ICommand<SaveOnboardingStepResult>;

    public record SaveOnboardingStepResult(bool Saved, string Status);

    public class SaveOnboardingStepCommandValidator : AbstractValidator<SaveOnboardingStepCommand>
    {
        public SaveOnboardingStepCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Data.LastStep).InclusiveBetween(1, 4);
            RuleFor(x => x.Data.Headline)
                .MaximumLength(60).When(x => x.Data.Headline != null);
        }
    }

    public class SaveOnboardingStepHandler(
        IOnboardingRepository onboardingRepo,
        IRoleService roleService,
        IUserRepository userRepo,
        IUnitOfWork<UserDbContext> unitOfWork,
        ILogger<SaveOnboardingStepHandler> logger)
        : ICommandHandler<SaveOnboardingStepCommand, SaveOnboardingStepResult>
    {
        public async Task<SaveOnboardingStepResult> Handle(
            SaveOnboardingStepCommand command, CancellationToken ct)
        {
            SaveOnboardingStepRequest data = command.Data;
            string email = command.Email;

            User user = await userRepo.GetUserByEmailAsync(email)
                       ?? throw new NotFoundException($"User '{email}' not found.");

            await unitOfWork.BeginTransactionAsync();
            try
            {
                InstructorOnboardingStatus status = new InstructorOnboardingStatus
                {
                    UserId = user.Id,
                    LastStep = data.LastStep,
                    Status = data.IsComplete
                                           ? OnboardingStatusValue.Completed
                                           : OnboardingStatusValue.InProgress,
                    YearsExperience = data.YearsExperience,
                    PriorCourses = data.PriorCourses,
                    Specialties = data.Specialties,
                    HasRecordingEquipment = data.HasRecordingEquipment,
                    CoursePlan = data.CoursePlan,
                    Headline = data.Headline,
                    Bio = data.Bio,
                    Language = data.Language,
                    PayoutMethod = data.PayoutMethod,
                };

                await onboardingRepo.UpsertAsync(status);

                // Assign role atomically with the Completed status write
                if (data.IsComplete)
                {
                    bool roleAdded = await roleService.AddToRoleAsync(email, "Instructor")
                                   ?? throw new InternalServerException("Role assignment failed.");

                    if (!roleAdded)
                        throw new InternalServerException($"Failed to assign Instructor role to {email}.");

                    logger.LogInformation("Instructor role assigned to {Email}", email);
                }

                await unitOfWork.CommitAsync();
                return new SaveOnboardingStepResult(true, status.Status);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                logger.LogError(ex, "Failed to save onboarding step for {Email}", email);
                throw;
            }
        }
    }
}
