using lms.shared.data.entities.coursemanagement;
using lms.shared.data.entities.coursemanagement.Content;

namespace lms.shared.data.repositories.coursemanagement
{
    public interface ICourseModuleRepository
    {
        Task<CourseModule?> GetByIdAsync(Guid id);
        Task<IEnumerable<CourseModule>> GetByCourseSectionIdAsync(Guid courseSectionId);
        Task<int> GetNextOrderForCourseSectionAsync(Guid courseSectionId);
        Task<IList<CourseModule>> AddAsync(Guid CourseSectionId, IList<CourseModule> CourseModules);
        Task<CourseModule> UpdateAsync(CourseModule module);
        Task DeleteAsync(CourseModule module);
        Task<CourseModule> AddDocumentModuleAsync(Guid courseSectionId, string title, string s3Key);
        Task<CourseModule> AddVideoModuleAsync(Guid courseSectionId, string title, string s3Key);
        Task<CourseModule> AddQuizModuleAsync(Guid courseSectionId, string title);
        Task<Quiz> AddQuizToModuleAsync(Guid moduleId, Quiz quiz);

    }
}
