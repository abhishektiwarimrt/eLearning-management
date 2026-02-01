using Microsoft.AspNetCore.Http;

namespace lms.shared.common.utilities
{
    public static class Utility
    {
        public static CourseContentType GetFileContentType(IFormFile file)
        {
            var strContentType = (CourseContentType)Enum.Parse(typeof(CourseContentType), file.ContentType.Replace("/", ""), true);
            return strContentType;
        }
    }
}
