using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System.Net;

namespace MoodReboot.Services
{
    public class ServiceSqs
    {
        private IAmazonSQS clientSQS;
        private string UrlQueue;

        public ServiceSqs(IAmazonSQS amazonSQS, IConfiguration configuration)
        {
            this.clientSQS = amazonSQS;
            this.UrlQueue = configuration.GetValue<string>("AWS:SQS:UrlQueue");
        }

        public async Task SendMessageAsync(string mensaje, string group)
        {
            string json = JsonConvert.SerializeObject(mensaje);
            SendMessageRequest request = new SendMessageRequest(this.UrlQueue, json);
            request.MessageGroupId = group;
            SendMessageResponse response = await this.clientSQS.SendMessageAsync(request);
            HttpStatusCode statusCode = response.HttpStatusCode;
        }
    }

}
