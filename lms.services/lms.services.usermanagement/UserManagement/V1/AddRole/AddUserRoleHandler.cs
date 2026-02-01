namespace lms.services.usermanagement.UserManagement.V1.AddRole
{
    public record AddUserRoleCommand(string? UserEmail, IList<Roles> UserRoles)
        : ICommand<AddUserRoleResult>;
    public record AddUserRoleResult(bool RoleAdded);

    public class AddUserRoleCommandValidator : AbstractValidator<AddUserRoleCommand>
    {
        public AddUserRoleCommandValidator()
        {
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage("Email is required!");
            RuleFor(x => x.UserRoles).Must(c => c.Count > 0).WithMessage("Roles are required"!);
            RuleFor(x => x.UserRoles).ForEach(r => r.IsInEnum().WithMessage("Invalid role specified!"));
        }
    }
    public class AddUserRoleHandler(IRoleService roleService, IUnitOfWork<UserDbContext> unitOfWork, ILogger<AddUserRoleHandler> logger)
        : ICommandHandler<AddUserRoleCommand, AddUserRoleResult>
    {
        public async Task<AddUserRoleResult> Handle(AddUserRoleCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.UserEmail == null)
                    throw new BadRequestException("Invalid UserEmail");

                var roleAdded = false;
                await unitOfWork.BeginTransactionAsync();

                foreach (var role in command.UserRoles)
                {
                    var isCreated = await roleService.AddToRoleAsync(command.UserEmail, role.ToString())
                    ?? throw new RegistrationFailedException("Failed to add roles to user");
                    if (isCreated)
                    {
                        var msg = $"User:{command.UserEmail} Role:{role.ToString()} Mapped!";
                        logger.LogInformation(msg);
                        roleAdded = true;
                    }
                    else
                    {
                        roleAdded = false;

                        var msg = $"Failed to add role:{role.ToString()} to user:{command.UserEmail}";
                        logger.LogError(msg);

                        throw new InternalServerException(msg);

                    }
                }

                await unitOfWork.CommitAsync();
                return new AddUserRoleResult(roleAdded);
            }
            catch (Exception ex)
            {
                var msg = $"Failed to add roles: {string.Join(", ", command.UserRoles)} to user: {command.UserEmail}\n{ex}";

                await unitOfWork.RollbackAsync();
                logger.LogError(msg, ex.StackTrace);
                throw;
            }
        }
    }
}
