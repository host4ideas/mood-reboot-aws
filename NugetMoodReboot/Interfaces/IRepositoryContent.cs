using NugetMoodReboot.Models;

namespace NugetMoodReboot.Interfaces
{
    public interface IRepositoryContent
    {
        public Task<int> GetMaxContentAsync();
        public Task CreateContentAsync(int contentGroupId, string text);
        public Task CreateContentFileAsync(int contentGroupId, int fileId);
        public Task DeleteContentAsync(int id);
        public Task<Content?> FindContentAsync(int id);
        public Task<List<Content>> GetContentByGroupAsync(int groupId);
        public Task UpdateContentAsync(int id, string? text = null, int? fileId = null);
    }
}
