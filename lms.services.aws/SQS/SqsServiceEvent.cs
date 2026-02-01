using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace lms.services.aws.SQS
{
    public class SqsServiceEvent : ISqsServiceEvent
    {

        private readonly IAmazonSQS _sqsClient;
        private readonly string _queueUrl;

        public SqsServiceEvent(IAmazonSQS sqsClient, IConfiguration configuration)
        {
            _sqsClient = sqsClient;
            _queueUrl = configuration["AWS:SQS:QueueUrl"] ?? throw new ArgumentNullException("AWS:SQS:QueueUrl");
        }

        public async Task<string> SendSQSMessageAsync(IFormFile file)
        {

            var fileDetails = new
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length
            };

            var message = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = JsonSerializer.Serialize(fileDetails)
            };

            var response = await _sqsClient.SendMessageAsync(message);

            return response.MessageId;
        }
    }
}
