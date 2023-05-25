using APIMoodReboot.Utils;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using MoodReboot.Hubs;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Services
{
    public class ServiceApiUsers
    {
        private readonly HelperApi helperApi;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceApiUsers(HelperApi helperApi, IHttpContextAccessor httpContextAccessor)
        {
            this.helperApi = helperApi;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ChatUserModel>> GetChatGroupUsersAsync(int chatGroupId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<ChatUserModel>>(Consts.ApiMessages + "/ChatGroupUsers/" + chatGroupId, token);
        }

        public async Task DeleteUserAsync(int userId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(Consts.ApiUsers + "/DeleteUser/" + userId, token);
        }

        public async Task<int> GetMaxFileAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<int>(Consts.ApiFiles + "/GetMaxFile", token);
        }

        public async Task AddUsersToChatAsync(int chatGroupId, List<int> userIds)
        {
            AddUsersChatApiModel model = new()
            {
                ChatGroupId = chatGroupId,
                UserIds = userIds
            };
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(Consts.ApiMessages + "/AddUsersToChat/", model, token);
        }

        public async Task ApproveUserAsync(int userId)
        {
            string tokenAuth = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PutAsync(
                request: Consts.ApiAdmin + "/ApproveUser/" + userId,
                body: null,
                token: tokenAuth
                );
        }

        public async Task ApproveUserAsync(int userId, string token)
        {
            await this.helperApi.PutAsync(
                request: Consts.ApiUsers + "/ApproveUserEmail/" + userId + "/" + token,
                body: null
                );
        }

        public async Task CreateMessageAsync(string token, int groupChatId, string userName, string? text = null, int? fileId = null)
        {
            CreateChatMessageApiModel model = new()
            {
                GroupChatId = groupChatId,
                UserName = userName,
                Text = text,
                FileId = fileId
            };

            await this.helperApi.PostAsync(
                request: Consts.ApiMessages + "/CreateMessage/",
                body: model,
                token: token
                );
        }

        public async Task ChangeEmailAsync(int userId, string token, string email)
        {
            await this.helperApi.PostAsync(Consts.ApiUsers + $"/ChangeEmail/{userId}/{token}/{email}", null, token);
        }

        public async Task<string> RequestChangeDataAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.PostAsync<string>(
                request: Consts.ApiUsers + "/RequestChangeData",
                body: null,
                token: token
                );
        }

        public async Task<string> RequestChangeDataAsync(int userId)
        {
            return await this.helperApi.PostAsync<string>(
                request: Consts.ApiUsers + "/RequestChangeData/" + userId,
                body: null
                );
        }

        public async Task ChangePasswordAsync(int userId, string token, string password)
        {
            await this.helperApi.PostAsync(Consts.ApiUsers + $"/ChangePassword/{userId}/{token}/{password}", null, token);
        }

        public async Task DeleteFileAsync(int fileId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(request: Consts.ApiFiles + "/DeleteFile/" + fileId, token: token);
        }

        public Task<AppFile?> FindFileAsync(int fileId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<AppFile>(Consts.ApiFiles + "/FindFile/" + fileId, token);
        }

        public Task<AppUser?> FindUserAsync(int userId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<AppUser>(Consts.ApiUsers + "/Profile", token);
        }

        public Task<List<AppUser>> GetAllUsersAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return this.helperApi.GetAsync<List<AppUser>>(Consts.ApiAdmin + "/Users", token);
        }

        public Task<int> GetMaxUserAsync()
        {
            return this.helperApi.GetAsync<int>(Consts.ApiUsers + "/GetMaxUser");
        }

        public async Task<List<Message>> GetMessagesByGroupAsync(int chatGroupId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<Message>>(Consts.ApiMessages + "/GetChatMessages/" + chatGroupId, token);
        }

        public async Task<List<AppUser>> GetPendingUsersAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<AppUser>>(Consts.ApiAdmin + "/UserRequests", token);
        }

        public async Task<List<Message>> GetUnseenMessagesAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<Message>>(Consts.ApiMessages + "/GetUnseenMessages", token);
        }

        public async Task<List<ChatGroup>> GetUserChatGroupsAsync()
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<ChatGroup>>(Consts.ApiMessages + "/GetUserChatGroups", token);
        }

        public async Task<List<ChatGroup>> GetUserChatGroupsAsync(string token)
        {
            return await this.helperApi.GetAsync<List<ChatGroup>>(Consts.ApiMessages + "/GetUserChatGroups", token);
        }

        public async Task UpdateFileAsync(UpdateFileApiModel model)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PutAsync(Consts.ApiFiles + "/UpdateFile", model, token);
        }

        public async Task<int> InsertFileAsync(CreateFileApiModel model)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.PostAsync<int>(Consts.ApiFiles + "/InsertFile", model, token);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await this.helperApi.GetAsync<bool>(Consts.ApiUsers + "/IsEmailAvailable/" + email);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return await this.helperApi.GetAsync<bool>(Consts.ApiUsers + $"/IsUsernameAvailable/'{username}'");
        }

        public async Task<string?> GetTokenAsync(string username, string password)
        {
            LoginModel model = new()
            {
                Password = password,
                Username = username,
            };

            return await this.helperApi.PostAsync<string>(Consts.ApiAuth + "/login", model);
        }

        public async Task<Tuple<string, AppUser>?> LoginUserAsync(string email, string password)
        {
            string? token = await this.GetTokenAsync(email, password);
            AppUser? user = await this.helperApi.GetAsync<AppUser>(Consts.ApiUsers + "/profile", token);

            if (token != null && user != null)
                return Tuple.Create(token, user);

            return null;
        }

        public async Task NewChatGroupAsync(List<int> userIds, string chatGroupName)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            CreateChatGroupModel model = new()
            {
                GroupName = chatGroupName,
                UserIds = userIds,
            };

            await this.helperApi.PostAsync(Consts.ApiMessages + "/CreateChatGroup", model, token);
        }

        public async Task<int> RegisterUserAsync(string nombre, string firstName, string lastName, string email, string password, string image)
        {
            SignUpModel model = new()
            {
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Path = image,
                Username = nombre
            };

            return await this.helperApi.PostAsync<int>(Consts.ApiUsers + "/RegisterUser", model);
        }

        public async Task RemoveChatGroupAsync(int chatGroupId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(Consts.ApiMessages + "/DeleteChatGroup/" + chatGroupId, token);
        }

        public async Task RemoveChatUserAsync(int userId, int chatGroupId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.DeleteAsync(Consts.ApiMessages + $"/RemoveUserFromChat/{userId}/{chatGroupId}", token);
        }

        public async Task<List<Tuple<string, int>>> SearchUsersAsync(string pattern)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.helperApi.GetAsync<List<Tuple<string, int>>>(Consts.ApiUsers + "/SearchUsers/" + pattern, token);
        }

        public async Task UpdateChatGroupAsync(int chatGroupId, string name)
        {
            ChatGroup model = new()
            {
                Id = chatGroupId,
                Name = name
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PutAsync(Consts.ApiMessages + "/UpdateChatGroup", model, token);
        }

        public async Task UpdateChatLastSeenAsync(int chatGroupId)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PutAsync(Consts.ApiMessages + "/UpdateChatLastSeen/" + chatGroupId, null, token);
        }

        public async Task UpdateUserBasicsAsync(string userName, string firstName, string lastName, string? image = null)
        {
            UpdateProfileApiModel model = new()
            {
                FirstName = firstName,
                LastName = lastName,
                Image = image,
                Username = userName
            };

            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PutAsync(Consts.ApiUsers + "/Profile", model, token);
        }

        public async Task UpdateUserEmailAsync(int userId, string email)
        {
            string tokenAuth = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(Consts.ApiUsers + $"/ChangeEmail/{userId}/{email}", null, tokenAuth);
        }

        public async Task UpdateUserPasswordAsync(int userId, string password)
        {
            string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.helperApi.PostAsync(Consts.ApiUsers + $"/ChangePassword/{userId}", null, token);
        }
    }
}
