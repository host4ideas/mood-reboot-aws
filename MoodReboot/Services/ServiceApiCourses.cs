using APIMoodReboot.Utils;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Services
{
    public class ServiceApiCourses
    {
        private readonly HelperApi helperApi;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceApiCourses(HelperApi helperApi, IHttpContextAccessor httpContextAccessor)
        {
            this.helperApi = helperApi;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task AddCourseEditorAsync(int courseId, int userId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(Consts.ApiCourses + $"/AddCourseEditor/{courseId}/{userId}", null, token);
        }

        public async Task<bool> AddCourseUserAsync(int courseId, int userId, bool isEditor)
        {
            CourseEnrollmentApiModel model = new()
            {
                CourseId = courseId,
                UserId = userId,
                IsEditor = isEditor
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            HttpResponseMessage response = await this.helperApi.PostAsync(Consts.ApiCourses + $"/CourseEnrollment", model, token);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddCourseUserAsync(int courseId, int userId, bool isEditor, string password)
        {
            CourseEnrollmentApiModel model = new()
            {
                CourseId = courseId,
                UserId = userId,
                IsEditor = isEditor,
                Password = password
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            HttpResponseMessage response = await this.helperApi.PostAsync(Consts.ApiCourses + $"/CourseEnrollment", model, token);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<CourseListView>> CenterCoursesListViewAsync(int centerId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<CourseListView>>(Consts.ApiCourses + "/CenterCourses/" + centerId, token);
        }

        public Task DeleteCourseAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.DeleteAsync(Consts.ApiCourses + "/DeleteCourse/" + id, token);
        }

        public Task<Course?> FindCourseAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<Course>(Consts.ApiCourses + "/FindCourse/" + id, token);
        }

        public async Task<UserCourse?> FindUserCourseAsync(int userId, int courseId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<UserCourse>(Consts.ApiCourses + "/FindUserCourse/" + courseId, token);
        }

        public Task<List<Course>> GetAllCoursesAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<Course>>(Consts.ApiCourses + "/GetAllCourses/", token);
        }

        public Task<List<CourseListView>> GetCenterCoursesAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<CourseListView>>(Consts.ApiCourses + "/CenterCourses/ + id", token);
        }

        public Task<List<CourseUsersModel>> GetCourseUsersAsync(int courseId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<CourseUsersModel>>(Consts.ApiCourses + "/GetCourseUsers/" + courseId, token);
        }

        public Task<List<CourseListView>> GetEditorCenterCoursesAsync(int userId, int centerId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<CourseListView>>(Consts.ApiCourses + $"/GetEditorCenterCourses/{userId}/{centerId}", token);
        }

        public Task<int> GetMaxCourseAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<int>(Consts.ApiCourses + "/GetMaxCourse", token);
        }

        public Task<List<CourseListView>> GetUserCoursesAsync(int id)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<CourseListView>>(Consts.ApiCourses + "/UserCourses/" + id, token);
        }

        public Task RemoveCourseEditorAsync(int courseId, int userId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.DeleteAsync(Consts.ApiCourses + $"/DeleteCourseEditor/{courseId}/{userId}", token);
        }

        public Task RemoveCourseUserAsync(int courseId, int userId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.DeleteAsync(Consts.ApiCourses + $"/DeleteCourseUser/{courseId}/{userId}", token);
        }

        public Task UpdateCourseAsync(int id, string description, string image, string name, bool isVisible)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            UpdateCourseApiModel model = new()
            {
                Id = id,
                Description = description,
                Image = image,
                Name = name,
                IsVisible = isVisible
            };

            return this.helperApi.PutAsync(Consts.ApiCourses + "/UpdateCourseAsync/", model, token);
        }

        public Task UpdateCourseVisibilityAsync(int courseId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.PutAsync(Consts.ApiCourses + "/UpdateCourseVisibility/" + courseId, null, token);
        }
    }
}
