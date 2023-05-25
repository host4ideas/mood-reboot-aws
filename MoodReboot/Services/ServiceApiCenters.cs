using APIMoodReboot.Utils;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Services
{
    public class ServiceApiCenters
    {
        private readonly HelperApi helperApi;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceApiCenters(HelperApi helperApi, IHttpContextAccessor httpContextAccessor)
        {
            this.helperApi = helperApi;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateCourseAsync(int centerId, string name, bool isVisible, string image, string description, string password)
        {
            CreateCourseApiModel model = new()
            {
                CenterId = centerId,
                Description = description,
                Image = image,
                IsVisible = isVisible,
                Name = name,
                Password = password
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            var response = await this.helperApi.PostAsync(Consts.ApiCourses + "/CreateCourse", model, token);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task AddEditorsCenterAsync(int centerId, List<int> userIds)
        {
            string request = Consts.ApiCenters + "/addcenterseditors/";

            AddCenterEditorsApiModel model = new()
            {
                CenterId = centerId,
                UserIds = userIds
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(request, model, token);
        }

        public async Task ApproveCenterAsync(Center center)
        {
            string request = Consts.ApiAdmin + "/approvecenter/" + center.Id;
            await this.helperApi.PutAsync(request, null);
        }

        public async Task CreateCenterAsync(string email, string name, string address, string telephone, string image, int director, bool approved)
        {
            string request = Consts.ApiAdmin + "/centerrequest";

            Center center = new()
            {
                Address = address,
                Approved = approved,
                Director = director,
                Email = email,
                Name = name,
                Image = image,
                Telephone = telephone,
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(request, center, token);
        }

        public async Task DeleteCenterAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(Consts.ApiCenters + "/DeleteCenter/" + id, token);
        }

        public Task<Center?> FindCenterAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<Center>(Consts.ApiCenters + "/FindCenter/" + id, token);
        }

        public Task<List<CenterListView>?> GetAllCentersAsync()
        {
            return this.helperApi.GetAsync<List<CenterListView>?>(Consts.ApiCenters + "/getcenters");
        }

        public Task<List<AppUser>?> GetCenterEditorsAsync(int centerId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<AppUser>?>(Consts.ApiCenters + "/CenterEditors/" + centerId, token);
        }

        public Task<int> GetMaxCenterAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<int>(Consts.ApiCenters + "/GetMaxCenter", token);
        }

        public Task<List<Center>?> GetPendingCentersAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<Center>?>(Consts.ApiAdmin + "/centerrequests", token);
        }

        public Task<List<CenterListView>?> GetUserCentersAsync(int userId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<CenterListView>?>(Consts.ApiCenters + "/UserCenters/" + userId, token);
        }

        public Task RemoveUserCenterAsync(int userId, int centerId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.DeleteAsync(Consts.ApiCenters + $"/RemoveUserCenter/{userId}/{centerId}", token);
        }

        public Task UpdateCenterAsync(int centerId, string email, string name, string address, string telephone, string image)
        {
            UpdateCenterApiModel model = new()
            {
                CenterId = centerId,
                Email = email,
                Name = name,
                Address = address,
                Telephone = telephone,
                Image = image
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.PutAsync(Consts.ApiCenters + "/updatecenter", model, token);
        }
    }
}
