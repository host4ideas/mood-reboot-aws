using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using MoodReboot.Models;
using System.Text;
using Azure.Security.KeyVault.Secrets;

namespace MoodReboot.Services
{
    public class ServiceContentModerator
    {
        private readonly ContentModeratorClient client;

        public ServiceContentModerator(IConfiguration configuration, SecretClient secretClient)
        {
            // Endpoint
            KeyVaultSecret moderatorEndpoint =
                secretClient.GetSecret("moderatorendpoint");
            string endpoint = moderatorEndpoint.Value;

            // Key
            KeyVaultSecret moderatorKey =
                secretClient.GetSecret("moderatorkey");
            string key = moderatorKey.Value;

            //string key = configuration.GetValue<string>("AzureKeys:ContentModeratorKey");
            //string endpoint = configuration.GetValue<string>("AzureKeys:ContentModeratorEndpoint");

            this.client = this.Authenticate(key, endpoint);
        }

        // Instantiate client objects with your endpoint and key
        private ContentModeratorClient Authenticate(string key, string endpoint)
        {
            ContentModeratorClient client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
            client.Endpoint = endpoint;

            return client;
        }

        /*
         * IMAGE MODERATION
         * This example moderates an image from URL.
         */
        public async Task<EvaluationData> ModerateImageAsync(string urlFile, string lang = "eng")
        {
            using (this.client)
            {
                var imageUrl = new BodyModel("URL", urlFile);

                var imageData = new EvaluationData
                {
                    ImageUrl = imageUrl.Value,

                    // Evaluate for adult and racy content.
                    ImageModeration =
                client.ImageModeration.EvaluateUrlInput("application/json", imageUrl, true)
                };
                Thread.Sleep(1000);

                // Detect and extract text.
                imageData.TextDetection =
                    client.ImageModeration.OCRUrlInput(lang, "application/json", imageUrl, true);
                Thread.Sleep(1000);

                // Detect faces.
                imageData.FaceDetection =
                    client.ImageModeration.FindFacesUrlInput("application/json", imageUrl, true);
                Thread.Sleep(1000);

                // Add results to Evaluation object
                return imageData;
            }
        }

        public Screen ModerateText(string text, string lang = "eng")
        {
            // Create a Content Moderator client and evaluate the text.
            using (this.client)
            {
                return
                    client.TextModeration.ScreenText("text/plain", new MemoryStream(Encoding.UTF8.GetBytes(text)), lang, true, true, null, true);
            }
        }
    }
}
