using APIMoodReboot.Utils;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Services
{
    public class ServiceApiContents
    {
        private readonly HelperApi helperApi;
        private IHttpContextAccessor httpContextAccessor;

        public ServiceApiContents(HelperApi helperApi, IHttpContextAccessor httpContextAccessor)
        {
            this.helperApi = helperApi;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpResponseMessage> CreateContentAsync(int contentGroupId, string text)
        {
            CreateContentModelApi model = new()
            {
                GroupId = contentGroupId,
                UnsafeHtml = text,
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            var response = await this.helperApi.PostAsync(Consts.ApiContent + "/AddContent", model, token);
            return response;
        }

        public async Task CreateContentFileAsync(int contentGroupId, int fileId)
        {
            // Find file in BBDD
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            AppFile? file = await this.helperApi.GetAsync<AppFile>(Consts.ApiFiles + "/FindFile/" + fileId, token);

            if (file != null)
            {
                CreateContentModelApi model = new()
                {
                    GroupId = contentGroupId,
                    File = file
                };

                await this.helperApi.PostAsync(Consts.ApiContent + "/AddContent", model, token);
            }
        }

        public async Task DeleteContentAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(Consts.ApiContent + "/DeleteContent/" + id, token);
        }

        public async Task<Content?> FindContentAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<Content>(Consts.ApiContent + "/FindContent/" + id, token);
        }

        public Task<List<Content>> GetContentByGroupAsync(int groupId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<Content>>(Consts.ApiContent + "/GetContentByGroup/" + groupId, token);
        }

        public Task<int> GetMaxContentAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<int>(Consts.ApiContent + "/GetMaxContent/", token);
        }

        public async Task UpdateContentAsync(int id, string? text = null, int? fileId = null)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            if (fileId != null)
            {
                // Find file in BBDD
                AppFile? file = await this.helperApi.GetAsync<AppFile>(Consts.ApiFiles + "/FindFile/" + fileId, token);

                UpdateContentApiModel model = new()
                {
                    ContentId = id,
                    File = file,
                };

                await this.helperApi.PutAsync(Consts.ApiContent + "/UpdateContent", model, token);
            }

            if (text != null)
            {
                UpdateContentApiModel model = new()
                {
                    ContentId = id,
                    UnsafeHtml = text,
                };

                await this.helperApi.PutAsync(Consts.ApiContent + "/UpdateContent", model, token);
            }
        }
    }
}
