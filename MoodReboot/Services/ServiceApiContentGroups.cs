using APIMoodReboot.Utils;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Services
{
    public class ServiceApiContentGroups
    {
        private readonly HelperApi helperApi;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceApiContentGroups(HelperApi helperApi, IHttpContextAccessor httpContextAccessor)
        {
            this.helperApi = helperApi;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateContentGroupAsync(string name, int courseId, bool isVisible = false)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(Consts.ApiContentGroups + $"/createcontentgroup/{name}/{courseId}/{isVisible}", null, token);
        }

        public async Task<List<ContentGroup>?> GetCourseContentGroupsAsync(int courseId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<ContentGroup>>(Consts.ApiContentGroups + "/CourseContentGroups/" + courseId, token);
        }

        public async Task DeleteContentGroupAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(Consts.ApiContentGroups + "/deletecontentgroup/" + id, token);
        }

        public Task<ContentGroup?> FindContentGroupAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateContentGroupAsync(int id, string name, bool isVisible)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PutAsync(Consts.ApiContentGroups + $"/updatecontentgroup/{id}/{name}/{isVisible}", null, token);
        }
    }
}
