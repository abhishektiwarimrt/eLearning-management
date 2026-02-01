using Microsoft.AspNetCore.Http;

namespace lms.services.aws.SQS
{
    public interface ISqsServiceEvent
    {
        /// <summary>
        /// Upload File to S3 bucket
        /// </summary>
        /// <param name="IFormFile">IFormFile</param>      
        /// <returns></returns>
        Task<string> SendSQSMessageAsync(IFormFile file);
    }
}
