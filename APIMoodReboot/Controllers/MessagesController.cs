using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NugetMoodReboot.Models;
using Microsoft.AspNetCore.Authorization;
using NugetMoodReboot.Interfaces;
using Newtonsoft.Json;

namespace APIMoodReboot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IRepositoryUsers repositoryUsers;

        public MessagesController(IRepositoryUsers repositoryUsers)
        {
            this.repositoryUsers = repositoryUsers;
        }

        [HttpGet("{chatGroupId}")]
        public async Task<List<ChatUserModel>> ChatGroupUsers(int chatGroupId)
        {
            return await this.repositoryUsers.GetChatGroupUsersAsync(chatGroupId);
        }

        [HttpGet("{chatGroupId}")]
        public async Task<ActionResult<List<Message>>> GetChatMessages(int chatGroupId)
        {
            return await this.repositoryUsers.GetMessagesByGroupAsync(chatGroupId);
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatGroup>>> GetUserChatGroups()
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            return await this.repositoryUsers.GetUserChatGroupsAsync(user.Id);
        }

        [HttpGet]
        public async Task<ActionResult<List<Message>>> GetUnseenMessages()
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            return await this.repositoryUsers.GetUnseenMessagesAsync(user.Id);
        }

        [HttpPut("{chatGroupId}")]
        public async Task<ActionResult> UpdateChatLastSeen(int chatGroupId)
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            await this.repositoryUsers.UpdateChatLastSeenAsync(chatGroupId, user.Id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateChatMessageApiModel model)
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            await this.repositoryUsers.CreateMessageAsync(user.Id, model.GroupChatId, model.UserName, model.Text, model.FileId);
            return CreatedAtAction(null, null);
        }

        [HttpPost]
        public async Task<ActionResult> CreateChatGroup(CreateChatGroupModel createChatGroup)
        {
            // Add current user to the list of users and set it as admin
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            createChatGroup.UserIds.Add(user.Id);

            // List without duplicates
            HashSet<int> userIdsNoDups = new(createChatGroup.UserIds);

            if (createChatGroup.UserIds.Count == 2)
            {
                await this.repositoryUsers.NewChatGroupAsync(userIdsNoDups);
            }
            else if (createChatGroup.UserIds.Count > 2)
            {
                await this.repositoryUsers.NewChatGroupAsync(userIdsNoDups, user.Id, createChatGroup.GroupName);
            }

            return CreatedAtAction(null, null);
        }

        [HttpPost]
        public async Task<ActionResult> AddUsersToChat(AddUsersChatApiModel model)
        {
            await this.repositoryUsers.AddUsersToChatAsync(model.ChatGroupId, model.UserIds);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateChatGroup(ChatGroup chatGroup)
        {
            await this.repositoryUsers.UpdateChatGroupAsync(chatGroup.Id, chatGroup.Name);
            return NoContent();
        }

        [HttpDelete("{chatGroupId}")]
        public async Task<ActionResult> DeleteChatGroup(int chatGroupId)
        {
            await this.repositoryUsers.RemoveChatGroupAsync(chatGroupId);
            return NoContent();
        }

        [HttpDelete("{userId}/{chatGroupId}")]
        public async Task<ActionResult> RemoveUserFromChat(int userId, int chatGroupId)
        {
            await this.repositoryUsers.RemoveChatUserAsync(userId, chatGroupId);
            return NoContent();
        }
    }
}
