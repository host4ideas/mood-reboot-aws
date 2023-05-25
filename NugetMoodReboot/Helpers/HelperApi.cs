using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace NugetMoodReboot.Helpers
{
    public class HelperApi
    {
        private readonly string _urlApi;
        private IHttpClientFactory HttpClientFactory { get; }

        public HelperApi(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this._urlApi = configuration.GetConnectionString("ApiMoodReboot");
            this.HttpClientFactory = httpClientFactory;
        }

        public async Task<T?> GetAsync<T>(string request)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }

            return default;
        }

        public async Task<T?> GetAsync<T>(string request, string token)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
            var response = await httpClient.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }

            return default;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string request)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await httpClient.DeleteAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string request, string token)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
            return await httpClient.DeleteAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync(string request, object? body)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (body == null)
            {
                return await httpClient.PutAsync(request, null);
            }
            return await httpClient.PutAsJsonAsync(request, body);
        }

        public async Task<HttpResponseMessage> PutAsync(string request, object? body, string token)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
            if (body == null)
            {
                return await httpClient.PutAsync(request, null);
            }
            return await httpClient.PutAsJsonAsync(request, body);
        }

        public async Task<HttpResponseMessage> PostAsync(string request, object? body)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (body == null)
            {
                return await httpClient.PostAsync(request, null);
            }
            return await httpClient.PostAsJsonAsync(request, body);
        }

        public async Task<T?> PostAsync<T>(string request, object? body)
        {
            using HttpClient client = HttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(this._urlApi);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync(request, body);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }

            return default;
        }

        public async Task<T?> PostAsync<T>(string request, object? body, string token)
        {
            using HttpClient client = HttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(this._urlApi);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

            var response = await client.PostAsJsonAsync(request, body);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }

            return default;
        }

        public async Task<HttpResponseMessage> PostAsync(string request, object? body, string token)
        {
            using HttpClient httpClient = HttpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(this._urlApi);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
            if (body == null)
            {
                return await httpClient.PostAsync(request, null);
            }
            return await httpClient.PostAsJsonAsync(request, body);
        }
    }
}
