namespace lms.services.coursemanagement.Services
{
    public interface ICourseSectionService
    {
        Task<CourseDto> GetCourseSectionByIdAsync(Guid CourseSectionId);
        Task<IEnumerable<CourseSectionDto>> GetAllCourseSectionsAsync(Guid CourseId);
        Task<IList<CourseSectionDto>> CreateCourseSectionsAsync(Guid CourseId, IList<CourseSectionDto> CourseSectionDtos);
        Task<IList<CourseSectionDto>> UpdateCourseSectionAsync(CourseSectionDto CourseSection);
        Task DeleteCourseSectionAsync(IList<Guid> CourseSectionIds);
    }
}