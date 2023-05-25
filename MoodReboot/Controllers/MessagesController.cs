using Microsoft.AspNetCore.Mvc;
using MoodReboot.Services;
using MvcCoreSeguridadEmpleados.Filters;
using NugetMoodReboot.Models;
using System.Security.Claims;

namespace MoodReboot.Controllers
{
    [AuthorizeUsers]
    public class MessagesController : Controller
    {
        private readonly ServiceApiUsers serviceUsers;

        public MessagesController(ServiceApiUsers serviceUsers)
        {
            this.serviceUsers = serviceUsers;
        }

        public IActionResult ChatErrorPartial(string errorMessage)
        {
            ViewData["ERROR"] = errorMessage;
            return View();
        }

        public async Task<IActionResult> ChatWindowPartial()
        {
            List<ChatGroup> groups = await this.serviceUsers.GetUserChatGroupsAsync();
            return PartialView("_ChatWindowPartial", groups);
        }

        public async Task<IActionResult> ChatNotificationsPartial()
        {
            List<Message> unseen = await this.serviceUsers.GetUnseenMessagesAsync();
            return PartialView("_ChatNotificationsPartial", unseen);
        }

        public async Task<List<Message>> GetChatMessages(int chatGroupId)
        {
            List<Message> messages = await this.serviceUsers.GetMessagesByGroupAsync(chatGroupId);
            return messages;
        }

        public async Task<List<Message>> GetUnseenMessages()
        {
            return await this.serviceUsers.GetUnseenMessagesAsync();
        }

        public async Task UpdateChatLastSeen(int chatGroupId)
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await this.serviceUsers.UpdateChatLastSeenAsync(chatGroupId);
        }

        public async Task CreateChatGroup(List<int> userIds, string? groupName = "NEW CHAT GROUP")
        {
            await this.serviceUsers.NewChatGroupAsync(userIds, groupName);
        }

        [HttpPost]
        public Task AddUsersToChat(int chatGroupId, List<int> userIds)
        {
            return this.serviceUsers.AddUsersToChatAsync(chatGroupId, userIds);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChatGroup(ChatGroup chatGroup)
        {
            await this.serviceUsers.UpdateChatGroupAsync(chatGroup.Id, chatGroup.Name);
            return RedirectToAction("Index", "Home");
        }

        public async Task DeleteChatGroup(int chatGroupId)
        {
            await this.serviceUsers.RemoveChatGroupAsync(chatGroupId);
        }

        public Task RemoveUserFromChat(int userId, int chatGroupId)
        {
            return this.serviceUsers.RemoveChatUserAsync(userId, chatGroupId);
        }
    }
}
