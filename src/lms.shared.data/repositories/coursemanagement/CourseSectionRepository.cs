using lms.shared.data.dbcontexts;
using lms.shared.data.entities.coursemanagement;

namespace lms.shared.data.repositories.coursemanagement
{
    public class CourseSectionRepository(CourseDbContext context) : ICourseSectionRepository
    {
        public async Task<IList<CourseSection>> AddAsync(IList<CourseSection> sections)
        {
            await context.CourseSections.AddRangeAsync(sections);
            await context.SaveChangesAsync();
            return sections;
        }

        public Task DeleteAsync(CourseSection section)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseSection>> GetByCourseIdAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<CourseSection> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetNextOrderForCourseAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<CourseSection> UpdateAsync(CourseSection section)
        {
            throw new NotImplementedException();
        }
    }
}
