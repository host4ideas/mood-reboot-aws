using MoodReboot.Models;
using System.Net.Http.Headers;

namespace MoodReboot.Services
{
    public class ServiceTextModeration
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly string UrlImageModerationService;

        public ServiceTextModeration(IHttpClientFactory httpClientFactory, SecretAWS secretAWS)
        {
            HttpClientFactory = httpClientFactory;
            this.UrlImageModerationService = secretAWS.ImageModerationUrl;
        }

        public async Task<string> ModerateText(string text)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string request = this.UrlImageModerationService + "api/ImageModeration";
            var response = await httpClient.PostAsJsonAsync(request, text);
            return await response.Content.ReadAsAsync<string>();
        }
    }
}
