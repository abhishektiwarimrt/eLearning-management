using lms.shared.data.dbcontexts;
using lms.shared.data.entities.coursemanagement;
using lms.shared.data.entities.coursemanagement.Content;
using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.repositories.coursemanagement
{
    public class CourseModuleRepository(CourseDbContext context)
        : ICourseModuleRepository
    {
        public async Task<IList<CourseModule>> AddAsync(Guid CourseSectionId, IList<CourseModule> CourseModules)
        {
            var creationTime = DateTime.UtcNow;
            foreach (var courseModule in CourseModules)
            {
                courseModule.CourseSectionId = CourseSectionId;
                courseModule.Id = Guid.NewGuid();
                courseModule.CreatedAt = creationTime;
            }
            await context.CourseModules.AddRangeAsync(CourseModules);
            return CourseModules;
        }

        public Task<CourseModule> AddDocumentModuleAsync(Guid courseSectionId, string title, string s3Key)
        {
            throw new NotImplementedException();
        }

        public Task<CourseModule> AddQuizModuleAsync(Guid courseSectionId, string title)
        {
            throw new NotImplementedException();
        }

        public Task<Quiz> AddQuizToModuleAsync(Guid moduleId, Quiz quiz)
        {
            throw new NotImplementedException();
        }

        public Task<CourseModule> AddVideoModuleAsync(Guid courseSectionId, string title, string s3Key)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(CourseModule module)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseModule>> GetByCourseSectionIdAsync(Guid courseSectionId)
        {
            throw new NotImplementedException();
        }

        public async Task<CourseModule?> GetByIdAsync(Guid id)
        {
            return await context.CourseModules
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> GetNextOrderForCourseSectionAsync(Guid courseSectionId)
        {
            throw new NotImplementedException();
        }

        public Task<CourseModule> UpdateAsync(CourseModule module)
        {
            context.CourseModules.Update(module);
            return Task.FromResult(module);
        }
    }
}
