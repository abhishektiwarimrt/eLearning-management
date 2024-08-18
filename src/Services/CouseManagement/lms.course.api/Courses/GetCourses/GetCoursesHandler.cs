
namespace lms.course.api.Courses.GetCourses
{
    public record GetCoursesQuery(Guid UserProfileId) : IQuery<GetCoursesResult>;
    public record GetCoursesResult(IEnumerable<Course> Courses);

    public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, GetCoursesResult>
    {
        public async Task<GetCoursesResult> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
