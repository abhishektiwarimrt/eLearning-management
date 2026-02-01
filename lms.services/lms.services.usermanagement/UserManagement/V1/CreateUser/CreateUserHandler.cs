namespace lms.services.usermanagement.UserManagement.V1.CreateUser
{
    public record CreateUserCommand(RegisterUserDto RegisterUser)
        : ICommand<AddUserRoleResult>;
    public record AddUserRoleResult(bool Registered);

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.RegisterUser.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.RegisterUser.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(x => x.RegisterUser.LastName).NotEmpty().WithMessage("Last Name is required");
            RuleFor(x => x.RegisterUser.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class CreateUserHandler(IUserService userService, IUnitOfWork<UserDbContext> unitOfWork, ILogger<CreateUserHandler> logger)
        : ICommandHandler<CreateUserCommand, AddUserRoleResult>
    {
        public async Task<AddUserRoleResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();

                var isCreated = await userService.CreateUserAsync(request.RegisterUser)
                    ?? throw new RegistrationFailedException("Failed to register user");
                if (isCreated)
                {
                    var count = await unitOfWork.CommitAsync();
                    logger.LogInformation($"User [{request.RegisterUser.Email}] Created Successfully! No. of Recored {count} inserted!");
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    logger.LogError($"User [{request.RegisterUser.Email}] Registration Failed!");
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
