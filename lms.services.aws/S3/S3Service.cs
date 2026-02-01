using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace lms.services.aws.S3
{
    public class S3ServiceEvent : IS3ServiceEvent
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;
        public S3ServiceEvent(IAmazonS3 s3Client, string bucketName)
        {
            _bucketName = bucketName;
            _s3Client = s3Client;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string key = $"{Guid.NewGuid()}_{file.FileName}";

            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = memoryStream,
                PartSize = 6291456,
                Key = key,
                BucketName = _bucketName,
                CannedACL = S3CannedACL.NoACL
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return key;
        }

        // new overload for background uploads
        public async Task<string> UploadFileAsync(Stream stream, string fileName)
        {
            string key = $"{Guid.NewGuid()}_{fileName}";
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = stream,
                PartSize = 6291456,
                Key = key,
                BucketName = _bucketName,
                CannedACL = S3CannedACL.NoACL
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest).ConfigureAwait(false);

            return key;
        }
    }
}
//using Amazon.S3;
//using Amazon.S3.Transfer;
//using Microsoft.AspNetCore.Http;

//namespace lms.services.aws.S3
//{
//    public class S3ServiceEvent : IS3ServiceEvent
//    {

//        private readonly string _bucketName;
//        // Create AWS S3 client object - credential is 1) access key, 2) secret access key
//        private readonly IAmazonS3 _s3Client;
//        public S3ServiceEvent(IAmazonS3 s3Client, string bucketName)
//        {
//            _bucketName = bucketName;

//            _s3Client = s3Client;
//        }

//        public async Task<string> UploadFileAsync(IFormFile file)
//        {
//            string key = $"{Guid.NewGuid()}_{file.FileName}";

//            await using var memoryStream = new MemoryStream();
//            await file.CopyToAsync(memoryStream);
//            var uploadRequest = new TransferUtilityUploadRequest()
//            {
//                InputStream = memoryStream,
//                PartSize = 6291456,
//                Key = key,
//                BucketName = _bucketName,
//                CannedACL = S3CannedACL.NoACL
//            };

//            //// initialise client
//            //// initialise the transfer/upload tools
//            var transferUtility = new TransferUtility(_s3Client);

//            //// initiate the file upload
//            await transferUtility.UploadAsync(uploadRequest);

//            return key;
//        }
//    }
//}


