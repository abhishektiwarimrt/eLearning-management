using Microsoft.AspNetCore.Http;

namespace lms.services.aws.S3
{
    public interface IS3ServiceEvent
    {
        /// <summary>
        /// Upload File to S3 bucket
        /// </summary>
        /// <param name="IFormFile">IFormFile</param>      
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> UploadFileAsync(Stream stream, string fileName);
    }
}
