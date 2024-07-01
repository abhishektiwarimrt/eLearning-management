

namespace lms.usermanagement.api.Profiles.GetProfiles
{
    public record GetUsersProfileQuery()
   : IQuery<GetUsersProfileResult>;
    public record GetUsersProfileResult(IEnumerable<UserProfile> UsersProfile);
    public class GetUsersProfileQueryHandler
        (IDocumentSession session)
        : IQueryHandler<GetUsersProfileQuery, GetUsersProfileResult>
    {
        public async Task<GetUsersProfileResult> Handle(GetUsersProfileQuery query, CancellationToken cancellationToken)
        {
            // logger.LogInformation("GetUsersProfileQueryHandler.Handle called with {@Query}", query);

            var usersProfile = await session.Query<UserProfile>().ToListAsync(cancellationToken);

            return new GetUsersProfileResult(usersProfile);
        }
    }
}

