using NugetMoodReboot.Models;

namespace NugetMoodReboot.Interfaces
{
    public interface IRepositoryCenters
    {
        public Task<int> GetMaxCenterAsync();
        public Task<List<CenterListView>> GetAllCentersAsync();
        public Task<List<Center>> GetPendingCentersAsync();
        public Task ApproveCenterAsync(Center center);
        public Task RemoveUserCenterAsync(int userId, int centerId);
        public Task AddEditorsCenterAsync(int centerId, List<int> userIds);
        public Task<List<CenterListView>> GetUserCentersAsync(int userId);
        public Task<List<AppUser>> GetCenterEditorsAsync(int centerId);
        public Task<Center?> FindCenterAsync(int id);
        public Task CreateCenterAsync(string email, string name, string address, string telephone, string image, int director, bool approved);
        public Task UpdateCenterAsync(int centerId, string email, string name, string address, string telephone, string image);
        public Task DeleteCenterAsync(int id);
    }
}
