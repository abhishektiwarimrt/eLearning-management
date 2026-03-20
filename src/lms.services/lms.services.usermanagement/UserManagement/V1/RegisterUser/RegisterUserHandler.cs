namespace lms.services.usermanagement.UserManagement.V1.CreateUser
{
    public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
        : ICommand<AddUserRoleResult>;
    public record AddUserRoleResult(bool Registered);

    public class CreateUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class RegisterUserHandler(IUserService userService, IUnitOfWork<UserDbContext> unitOfWork, ILogger<RegisterUserHandler> logger)
        : ICommandHandler<RegisterUserCommand, AddUserRoleResult>
    {
        public async Task<AddUserRoleResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();

                bool isCreated = await userService.CreateUserAsync(request)
                    ?? throw new RegistrationFailedException("Failed to register user");
                if (isCreated)
                {
                    int count = await unitOfWork.CommitAsync();
                    logger.LogInformation($"User [{request.Email}] Created Successfully! No. of Recored {count} inserted!");
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    logger.LogError($"User [{request.Email}] Registration Failed!");
                }
                return new AddUserRoleResult(isCreated);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                logger.LogError($"Failed to create user \n{ex}", ex.StackTrace);
                throw;
            }
        }
    }
}
