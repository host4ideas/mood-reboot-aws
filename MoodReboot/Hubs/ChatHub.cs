using Microsoft.AspNetCore.SignalR;
using MoodReboot.Services;
using NugetMoodReboot.Models;
using System.Linq;
using System.Security.Claims;

namespace MoodReboot.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ServiceApiUsers serviceUsers;
        private readonly ServiceContentModerator contentModerator;

        public ChatHub(ServiceApiUsers serviceUsers, ServiceContentModerator contentModerator)
        {
            this.serviceUsers = serviceUsers;
            this.contentModerator = contentModerator;
        }

        public async Task SendMessage(int userId, int groupChatId, string userName, string text, string fileId)
        {
            await Clients.All.SendAsync("ReceiveMessage", userName, text);
        }

        public async Task SendMessageToGroup(string userId, string groupChatId, string userName, string text)
        {
            var result = this.contentModerator.ModerateText(text);
            text = result.AutoCorrectedText;
            var reviewNeeded = result.Classification.ReviewRecommended;

            if (reviewNeeded == true)
            {
                text = "Moderated message";
            }

            // Send message to group
            await Clients.Group(groupChatId.ToString()).SendAsync(
                "ReceiveMessageGroup",
                userName,
                groupChatId,
                DateTime.Now,
                text);

            // Store the mesage in the DDBB
            string token = Context.User.FindFirstValue("TOKEN");
            await this.serviceUsers.CreateMessageAsync(
                token: token,
                groupChatId: int.Parse(groupChatId),
                userName: userName,
                text: text);
        }

        public async Task AddToGroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("GroupNotification", $"{userName} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("GroupNotification", $"{userName} has left the group {groupName}.");
        }

        public override Task OnConnectedAsync()
        {
            // Check if the user is logged
            if (Context.User.Identity.IsAuthenticated == true)
            {
                string userName = Context.User.FindFirstValue(ClaimTypes.Name);

                string token = Context.User.FindFirstValue("TOKEN");

                // If the user is logged in add it to its chat groups
                List<ChatGroup> groups = this.serviceUsers.GetUserChatGroupsAsync(token).Result;

                foreach (ChatGroup group in groups)
                {
                    this.AddToGroup(group.Id.ToString(), userName).Wait();
                }
            }

            return base.OnConnectedAsync();
        }
    }
}
