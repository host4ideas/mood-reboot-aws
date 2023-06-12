using MoodReboot.Models;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;
using System.Net.Http.Headers;

namespace MoodReboot.Services {
    public class ServiceMail {
        private readonly HelperApi helperApi;
        private readonly string UrlEmailService;
        private readonly IHttpClientFactory HttpClientFactory;

        public ServiceMail(HelperApi helperApi, IHttpClientFactory httpClientFactory, SecretAWS secretAWS) {
            this.helperApi = helperApi;
            this.HttpClientFactory = httpClientFactory;
            this.UrlEmailService = secretAWS.EmailServiceUrl;
        }

        public async Task<HttpResponseMessage> PostAsync(string request, object? body) {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (body == null) {
                return await httpClient.PostAsync(this.UrlEmailService + request, null);
            }
            var response = await httpClient.PostAsJsonAsync(this.UrlEmailService + request, body);
            return response;
        }

        public async Task SendMailAsync(string to, string message, string subject, string baseUrl, List<MailLink> links) {

            string request = "/api/email";
            var response = await this.PostAsync(request, new SendEmailModel() {
                To = to,
                Message = message,
                Subject = subject,
                BaseUrl= baseUrl,
                Links = links
            });
        }

        public async Task SendMailAsync(string to, string message, string subject, string baseUrl) {

            string request = "/api/email";
            var response = await this.PostAsync(request, new SendEmailModel() {
                To = to,
                Message = message,
                Subject = subject,
                BaseUrl = baseUrl
            });
        }
    }
}
