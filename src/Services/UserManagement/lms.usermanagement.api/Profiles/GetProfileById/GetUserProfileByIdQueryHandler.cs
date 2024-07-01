
namespace lms.usermanagement.api.Profiles.GetProfileById
{
    public record GetUserProfileByIdQuery(Guid id) :
        IQuery<GetUserProfileByIdResult>;

    public record GetUserProfileByIdResult(UserProfile UserProfile);
    public class GetUserProfileByIdQueryHandler(IDocumentSession session)
        : IQueryHandler<GetUserProfileByIdQuery, GetUserProfileByIdResult>
    {
        public async Task<GetUserProfileByIdResult> Handle(GetUserProfileByIdQuery query, CancellationToken cancellationToken)
        {
            var userProfile = await session.LoadAsync<UserProfile>(query.id, cancellationToken);

            if (userProfile is null)
            {
                throw new UserProfileNotFoundException();
            }

            return new GetUserProfileByIdResult(userProfile);
        }
    }
}
