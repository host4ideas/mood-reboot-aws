using NugetMoodReboot.Models;

namespace NugetMoodReboot.Interfaces
{
    public interface IRepositoryContentGroups
    {
        public Task<ContentGroup?> FindContentGroupAsync(int id);
        public Task<List<ContentGroup>> GetCourseContentGroupsAsync(int courseId);
        public Task UpdateContentGroupAsync(int id, string name, bool isVisible);
        public Task CreateContentGroupAsync(string name, int courseId, bool isVisible = false);
        public Task DeleteContentGroupAsync(int id);
    }
}
