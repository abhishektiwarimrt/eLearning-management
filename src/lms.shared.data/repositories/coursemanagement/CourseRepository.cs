using lms.shared.data.dbcontexts;
using lms.shared.data.entities.coursemanagement;
using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.repositories.coursemanagement
{
    public class CourseRepository(CourseDbContext context) : ICourseRepository
    {
        public async Task<Course> AddAsync(Course course)
        {
            var creationTime = DateTime.UtcNow;
            course.Id = Guid.NewGuid();
            course.Sections.ForEach(section =>
            {
                section.CreatedAt = creationTime;
                section.Id = Guid.NewGuid();
            });

            await context.Courses.AddAsync(course);
            await context.SaveChangesAsync();

            return course;
        }

        public Task DeleteAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Course>> GetByCreatorIdAsync(int creatorId)
        {
            throw new NotImplementedException();
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await context.Courses
                .Include(x => x.Sections).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Course> UpdateAsync(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
