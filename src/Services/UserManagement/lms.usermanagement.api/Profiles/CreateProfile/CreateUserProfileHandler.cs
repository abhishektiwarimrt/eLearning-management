

using FluentValidation;

namespace lms.usermanagement.api.Profiles.CreateProfile
{
    public record CreateUserProfileCommand(string FirstName, string LastName, string Title, string Address, string City, string Country)
    : IRequest<CreateUserProfileResult>;
    public record CreateUserProfileResult(Guid Id);

    public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
    {
        public CreateUserProfileCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required");
        }
    }

    internal class CreateUserProfileCommandHandler(IDocumentSession session, IValidator<CreateUserProfileCommand> validator)
        : IRequestHandler<CreateUserProfileCommand, CreateUserProfileResult>
    {
        public async Task<CreateUserProfileResult> Handle(CreateUserProfileCommand command, CancellationToken cancellationToken)
        {

            var result = await validator.ValidateAsync(command, cancellationToken);
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            if (errors.Any())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException(errors.FirstOrDefault());
            }

            //Create Entity from command object
            //Save Entity
            //Retrun result

            var userProfile = new UserProfile
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Title = command.Title,
                Address = command.Address,
                City = command.City,
                Country = command.Country
            };

            //Save Entity
            session.Store(userProfile);
            await session.SaveChangesAsync(cancellationToken);

            //Return result
            return new CreateUserProfileResult(userProfile.Id);
        }
    }
}
