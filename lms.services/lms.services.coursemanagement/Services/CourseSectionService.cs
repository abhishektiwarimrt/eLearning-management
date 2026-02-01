


namespace lms.services.coursemanagement.Services
{
    public class CourseSectionService(ICourseRepository courseRepository, ICourseSectionRepository courseSectionRepository) : ICourseSectionService
    {
        public async Task<IList<CourseSectionDto>> CreateCourseSectionsAsync(Guid CourseId, IList<CourseSectionDto> CourseSectionDtos)
        {
            var course = await courseRepository.GetByIdAsync(CourseId) ?? throw new NotFoundException($"Invalid CourserId[{CourseId}]!");
            var courseSections = CourseSectionDtos.Adapt<IList<CourseSection>>();

            foreach (var courseSection in courseSections)
            {
                courseSection.CourseId = course.Id;
            }
            var result = await courseSectionRepository.AddAsync(courseSections);
            var resultDto = result.Adapt<IList<CourseSectionDto>>();
            return resultDto;
        }

        public Task DeleteCourseSectionAsync(IList<Guid> CourseSectionIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseSectionDto>> GetAllCourseSectionsAsync(Guid CourseId)
        {
            throw new NotImplementedException();
        }

        public Task<CourseDto> GetCourseSectionByIdAsync(Guid CourseSectionId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CourseSectionDto>> UpdateCourseSectionAsync(CourseSectionDto CourseSection)
        {
            throw new NotImplementedException();
        }
    }
}
