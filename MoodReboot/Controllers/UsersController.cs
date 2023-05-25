using Microsoft.AspNetCore.Mvc;
using MoodReboot.Helpers;
using MoodReboot.Services;
using MvcCoreSeguridadEmpleados.Filters;
using MvcLogicApps.Services;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;
using System.Security.Claims;

namespace MoodReboot.Controllers
{
    public class UsersController : Controller
    {
        private readonly ServiceApiUsers serviceUsers;
        private readonly HelperFileAzure helperFile;
        private readonly ServiceLogicApps serviceLogicApps;

        public UsersController(ServiceApiUsers serviceUsers, HelperFileAzure helperFile, ServiceLogicApps serviceLogicApps)
        {
            this.serviceUsers = serviceUsers;
            this.helperFile = helperFile;
            this.serviceLogicApps = serviceLogicApps;
        }

        public async Task<List<Tuple<string, int>>> SearchUsers(string pattern)
        {
            return await this.serviceUsers.SearchUsersAsync(pattern);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Profile()
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            AppUser? user = await this.serviceUsers.FindUserAsync(userId);

            user.Image = await this.helperFile.GetBlobUriAsync(Containers.ProfileImages, user.Image);

            return View(user);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> Profile(string userName, string firstName, string lastName, IFormFile image)
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (image != null && image.Length > 0)
            {
                string fileName = "image_" + userId;
                await this.helperFile.UpdateFileAsync(image, Containers.ProfileImages, FileTypes.Image, fileName);
                await this.serviceUsers.UpdateUserBasicsAsync(userName, firstName, lastName, fileName);
                return RedirectToAction("Profile", new { userId });
            }

            await this.serviceUsers.UpdateUserBasicsAsync(userName, firstName, lastName);
            return RedirectToAction("Profile", new { userId });
        }

        /// <summary>
        /// Approve user register request
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IActionResult> ApproveUserEmail(int userId, string token)
        {
            await this.serviceUsers.ApproveUserAsync(userId, token);
            return RedirectToAction("Logout", "Managed");
        }

        #region CHANGE EMAIL

        public async Task<IActionResult> ChangeEmail(int userId, string token)
        {
            AppUser? user = await this.serviceUsers.FindUserAsync(userId);

            if (user != null)
            {
                ViewData["ERROR"] = "Petición invalida";
            }
            else
            {
                ViewData["TOKEN"] = token;
                ViewData["USERID"] = userId;
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(int userId, string token, string email)
        {
            await this.serviceUsers.ChangeEmailAsync(userId, token, email);
            return RedirectToAction("Logout", "Managed");
        }

        [AuthorizeUsers]
        public async Task RequestChangeEmail(int userId, string email)
        {
            string token = await this.serviceUsers.RequestChangeDataAsync();
            string url = Url.Action("ChangeEmail", "Users", new { userId, token })!;

            List<MailLink> links = new()
            {
                new MailLink()
                {
                    LinkText = "Confirmar cambio email",
                    Link = url
                }
            };
            string protocol = HttpContext.Request.IsHttps ? "https" : "http";
            string domainName = HttpContext.Request.Host.Value.ToString();
            string baseUrl = protocol + domainName;
            await this.serviceLogicApps.SendMailAsync(email, "Cambio de datos", "Se ha solicitado una petición para cambiar el correo electrónico de la cuenta asociada. Pulsa el siguiente enlace para confirmarla. Una vez cambiada deberás de iniciar sesión con el nuevo correo electónico, si surge cualquier problema o tienes alguna duda, contáctanos a: moodreboot@gmail.com. <br/><br/> Si no eres solicitante no te procupes, la petición será cancelada en un período de 24hrs.", links, baseUrl);
        }

        #endregion

        #region CHANGE PASSWORD

        public async Task<IActionResult> ChangePassword(int userId, string token)
        {
            AppUser? user = await this.serviceUsers.FindUserAsync(userId);

            if (user != null)
            {
                ViewData["ERROR"] = "Petición invalida";
            }
            else
            {
                ViewData["TOKEN"] = token;
                ViewData["USERID"] = userId;
            }

            return View(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangePassword(int userId, string token, string password)
        {
            await this.serviceUsers.ChangePasswordAsync(userId, token, password);
            return RedirectToAction("Logout", "Managed");
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task RequestChangePassword(int userId, string email)
        {
            string token = await this.serviceUsers.RequestChangeDataAsync();
            string url = Url.Action("ChangePassword", "Users", new { userId, token })!;

            List<MailLink> links = new()
            {
                new MailLink()
                {
                    LinkText = "Confirmar cambio de contraseña",
                    Link = url
                }
            };
            string protocol = HttpContext.Request.IsHttps ? "https" : "http";
            string domainName = HttpContext.Request.Host.Value.ToString();
            string baseUrl = protocol + domainName;
            await this.serviceLogicApps.SendMailAsync(email, "Cambio de datos", "Se ha solicitado una petición para cambiar la contraseña de la cuenta asociada. Pulsa el siguiente enlace para confirmarla. Una vez cambiada deberás de iniciar sesión con el nuevo correo electónico, si surge cualquier problema o tienes alguna duda, contáctanos a: moodreboot@gmail.com. <br/><br/> Si no eres solicitante no te procupes, la petición será cancelada en un período de 24hrs.", links, baseUrl);
        }

        #endregion
    }
}
