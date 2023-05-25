using NugetMoodReboot.Models;

namespace NugetMoodReboot.Interfaces
{
    public interface IRepositoryCourses
    {
        public Task<int> GetMaxCourseAsync();
        public Task<List<Course>> GetAllCoursesAsync();
        public Task<Course?> FindCourseAsync(int id);
        public Task<UserCourse?> FindUserCourseAsync(int userId, int courseId);
        public Task<List<CourseListView>> GetUserCoursesAsync(int id);
        public Task<List<CourseListView>> GetCenterCoursesAsync(int id);
        public Task<List<CourseUsersModel>> GetCourseUsersAsync(int courseId);
        public Task<List<CourseListView>> GetEditorCenterCoursesAsync(int userId, int centerId);
        public Task<List<CourseListView>> CenterCoursesListViewAsync(int centerId);
        public Task RemoveCourseUserAsync(int courseId, int userId);
        public Task RemoveCourseEditorAsync(int courseId, int userId);
        public Task AddCourseEditorAsync(int courseId, int userId);
        public Task<bool> AddCourseUserAsync(int courseId, int userId, bool isEditor);
        public Task<bool> AddCourseUserAsync(int courseId, int userId, bool isEditor, string password);
        public Task DeleteCourseAsync(int id);
        public Task UpdateCourseVisibilityAsync(int courseId);
        public Task UpdateCourseAsync(int id, string description, string image, string name, Boolean isVisible);
    }
}
