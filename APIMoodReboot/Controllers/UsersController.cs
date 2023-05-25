using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NugetMoodReboot.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryUsers repositoryUsers;

        public UsersController(IRepositoryUsers repositoryUsers)
        {
            this.repositoryUsers = repositoryUsers;
        }

        [HttpGet("{pattern}")]
        [Authorize]
        public async Task<ActionResult<List<Tuple<string, int>>>> SearchUsers(string pattern)
        {
            return await this.repositoryUsers.SearchUsersAsync(pattern);
        }

        [HttpPost]
        public async Task<int> RegisterUser(SignUpModel model)
        {
            return await this.repositoryUsers.RegisterUserAsync(model.Username, model.FirstName, model.LastName, model.Email, model.Password, model.Path);
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetMaxUser()
        {
            return await this.repositoryUsers.GetMaxUserAsync();
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<bool>> IsEmailAvailable(string email)
        {
            return await this.repositoryUsers.IsEmailAvailableAsync(email);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<bool>> IsUsernameAvailable(string username)
        {
            username = username.Replace("'", string.Empty);
            return await this.repositoryUsers.IsUsernameAvailableAsync(username);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<AppUser> Profile()
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Profile(UpdateProfileApiModel updateProfile)
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            if (updateProfile.Image != null)
            {
                await this.repositoryUsers.UpdateUserBasicsAsync(user.Id, updateProfile.Username, updateProfile.FirstName, updateProfile.LastName, updateProfile.Image);
                return NoContent();
            }

            await this.repositoryUsers.UpdateUserBasicsAsync(user.Id, updateProfile.Username, updateProfile.FirstName, updateProfile.LastName);
            return NoContent();
        }

        [HttpPut("{userId}/{token}")]
        public async Task<ActionResult> ApproveUserEmail(int userId, string token)
        {
            UserAction? userAction = await this.repositoryUsers.FindUserActionAsync(userId, token);

            if (userAction != null)
            {
                DateTime limitDate = userAction.RequestDate.AddHours(24);
                if (DateTime.Now > limitDate)
                {
                    await this.repositoryUsers.RemoveUserActionAsync(userAction);
                }
                else
                {
                    await this.repositoryUsers.RemoveUserActionAsync(userAction);
                    await this.repositoryUsers.ApproveUserAsync(userId);
                }
            }
            return NoContent();
        }

        #region CHANGE EMAIL

        [HttpPost("{userId}/{token}/{email}")]
        [Authorize]
        public async Task<ActionResult> ChangeEmail(int userId, string token, string email)
        {
            UserAction? userAction = await this.repositoryUsers.FindUserActionAsync(userId, token);

            if (userAction != null)
            {
                DateTime limitDate = userAction.RequestDate.AddHours(24);
                // Expired request - passed 24hrs
                if (DateTime.Now > limitDate)
                {
                    await this.repositoryUsers.RemoveUserActionAsync(userAction);
                    return NoContent();
                }
                else
                {
                    await this.repositoryUsers.RemoveUserActionAsync(userAction);
                    await this.repositoryUsers.UpdateUserEmailAsync(userId, email);
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> RequestChangeData()
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            string token = await this.repositoryUsers.CreateUserActionAsync(user.Id);
            return Ok(token);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> RequestChangeData(int userId)
        {
            string token = await this.repositoryUsers.CreateUserActionAsync(userId);
            return Ok(token);
        }

        #endregion

        #region CHANGE PASSWORD

        [HttpPost("{userId}/{token}/{password}")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(int userId, string token, string password)
        {
            UserAction? userAction = await this.repositoryUsers.FindUserActionAsync(userId, token);

            if (userAction != null)
            {
                DateTime limitDate = userAction.RequestDate.AddHours(24);
                // Expired request - passed 24hrs
                if (DateTime.Now > limitDate)
                {
                    await this.repositoryUsers.RemoveUserActionAsync(userAction);
                    return NoContent();
                }
                else
                {
                    await this.repositoryUsers.RemoveUserActionAsync(userAction);
                    await this.repositoryUsers.UpdateUserPasswordAsync(userId, password);
                }
            }
            return NoContent();
        }

        #endregion
    }
}
