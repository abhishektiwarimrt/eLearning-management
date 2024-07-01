
namespace lms.usermanagement.api.Profiles.DeleteProfile
{
    public record DeleteUserProfileCommand(Guid id)
        : ICommand<DeleteUserProfileResult>;

    public record DeleteUserProfileResult(bool isSuccess);
    public class DeleteUserProfileQueryHandler(IDocumentSession session)
        : ICommandHandler<DeleteUserProfileCommand, DeleteUserProfileResult>
    {
        public async Task<DeleteUserProfileResult> Handle(DeleteUserProfileCommand query, CancellationToken cancellationToken)
        {
            session.Delete<UserProfile>(query.id);
            await session.SaveChangesAsync(cancellationToken);

            return new DeleteUserProfileResult(true);
        }
    }
}
