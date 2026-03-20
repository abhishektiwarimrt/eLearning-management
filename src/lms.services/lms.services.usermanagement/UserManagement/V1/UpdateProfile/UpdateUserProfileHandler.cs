namespace lms.services.usermanagement.UserManagement.V1.UpdateUserProfile
{
    // ── Request DTO (shared between web layer and service layer) ─────────────
    // Defined here and registered in global.cs so it's available project-wide.
    public record UpdateUserProfileRequest(
        string FirstName,
        string LastName,
        string? Headline = null,
        string? Bio = null,
        string? Website = null,
        string? LinkedInUrl = null,
        string? TwitterHandle = null,
        string? Language = null);

    // ── MediatR command ───────────────────────────────────────────────────────
    public record UpdateUserProfileCommand(string Email, UpdateUserProfileRequest Data)
        : ICommand<bool>;

    // ── Validator ─────────────────────────────────────────────────────────────
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Data.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name must be 100 characters or less.");

            RuleFor(x => x.Data.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name must be 100 characters or less.");

            RuleFor(x => x.Data.Headline)
                .MaximumLength(60).WithMessage("Headline must be 60 characters or less.")
                .When(x => x.Data.Headline != null);

            RuleFor(x => x.Data.Bio)
                .MaximumLength(2000).WithMessage("Bio must be 2000 characters or less.")
                .When(x => x.Data.Bio != null);

            RuleFor(x => x.Data.Website)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Website must be a valid URL.")
                .When(x => !string.IsNullOrEmpty(x.Data.Website));

            RuleFor(x => x.Data.LinkedInUrl)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("LinkedIn URL must be a valid URL.")
                .When(x => !string.IsNullOrEmpty(x.Data.LinkedInUrl));
        }
    }

    // ── Handler ───────────────────────────────────────────────────────────────
    public class UpdateUserProfileHandler(
        IUserService userService,
        IUnitOfWork<UserDbContext> unitOfWork,
        ILogger<UpdateUserProfileHandler> logger)
        : ICommandHandler<UpdateUserProfileCommand, bool>
    {
        public async Task<bool> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();

                bool updated = await userService.UpdateProfileAsync(command.Email, command.Data);

                if (updated)
                {
                    await unitOfWork.CommitAsync();
                    logger.LogInformation("Profile committed for {Email}", command.Email);
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    logger.LogWarning("Profile update returned false for {Email} — rolling back", command.Email);
                }

                return updated;
            }
            catch (NotFoundException)
            {
                await unitOfWork.RollbackAsync();
                throw; // Let CustomExceptionHandler return 404
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                logger.LogError(ex, "Unexpected error updating profile for {Email}", command.Email);
                throw;
            }
        }
    }
}
