using AngleSharp.Text;
using MoodReboot.Helpers;
using MoodReboot.Models;
using System.Net.Http.Headers;

namespace MoodReboot.Services
{
    public class ServiceImageModeration
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly string UrlTextModerationService;

        public ServiceImageModeration(IHttpClientFactory httpClientFactory, SecretAWS secretAWS)
        {
            HttpClientFactory = httpClientFactory;
            this.UrlTextModerationService = secretAWS.ImageModerationUrl;
        }

        public async Task<bool> ModerateImageAsync(Containers container, string objectKey)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string request = this.UrlTextModerationService + "/api/TextModeration";
            var response = await httpClient.PostAsJsonAsync(request, new
            {
                Container = container,
                ObjectKey = objectKey
            });
            var data = await response.Content.ReadAsAsync<string>();
            if (data == null)
                return true;
            return data.ToBoolean();
        }
    }
}
