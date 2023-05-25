using NugetMoodReboot.Models;

namespace NugetMoodReboot.Interfaces
{
    public interface IRepositoryUsers
    {
        // Users
        public Task<AppUser?> FindUserAsync(int userId);
        public Task ApproveUserAsync(AppUser user);
        public Task ApproveUserAsync(int userId);
        public Task<List<AppUser>> GetPendingUsersAsync();
        public Task<bool> IsEmailAvailableAsync(string email);
        public Task<bool> IsUsernameAvailableAsync(string userName);
        public Task<List<Tuple<string, int>>> SearchUsersAsync(string pattern);
        public Task<int> GetMaxUserAsync();
        public Task<List<AppUser>> GetAllUsersAsync();
        public Task<AppUser?> LoginUserAsync(string email, string password);
        public Task<int> RegisterUserAsync(string nombre, string firstName, string lastName, string email, string password, string image);
        public Task DeleteUserAsync(int userId);
        public Task UpdateUserBasicsAsync(int userId, string userName, string firstName, string lastName, string? image = null);
        public Task UpdateUserEmailAsync(int userId, string email);
        public Task UpdateUserPasswordAsync(int userId, string password);
        public Task DeactivateUserAsync(int userId);
        // User Action
        public Task<UserAction?> FindUserActionAsync(int userId, string token);
        public Task RemoveUserActionAsync(UserAction userAction);
        public Task<string> CreateUserActionAsync(int userId);
        // Files
        public Task<AppFile?> FindFileAsync(int fileId);
        public Task<int> GetMaxFileAsync();
        public Task DeleteFileAsync(int fileId);
        public Task UpdateFileAsync(int fileId, string fileName, string mimeType);
        public Task UpdateFileAsync(int fileId, string fileName, string mimeType, int userId);
        public Task<int> InsertFileAsync(string name, string mimeType);
        public Task<int> InsertFileAsync(string name, string mimeType, int userId);
        // Messages
        public Task<List<ChatGroup>> GetUserChatGroupsAsync(int userId);
        public Task<List<Message>> GetMessagesByGroupAsync(int chatGroupId);
        public Task CreateMessageAsync(int userId, int groupChatId, string userName, string? text = null, int? fileId = null);
        public Task DeleteMessageAsync(int id);
        public Task<List<Message>> GetUnseenMessagesAsync(int userId);
        public Task UpdateChatLastSeenAsync(int chatGroupId, int userId);
        public Task NewChatGroupAsync(HashSet<int> userIdsNoDups);
        public Task<int> NewChatGroupAsync(HashSet<int> userIdsNoDups, int adminUserId, string chatGroupName);
        public Task RemoveChatGroupAsync(int chatGroupId);
        public Task UpdateChatGroupAsync(int chatGroupId, string name);
        public Task<List<ChatUserModel>> GetChatGroupUsersAsync(int chatGroupId);
        public Task RemoveChatUserAsync(int userId, int chatGroupId);
        public Task AddUsersToChatAsync(int chatGroupId, List<int> userIds);
    }
}
