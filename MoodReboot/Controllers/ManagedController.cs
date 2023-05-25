using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MoodReboot.Helpers;
using MoodReboot.Services;
using MvcLogicApps.Services;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;
using System.Security.Claims;

namespace MoodReboot.Controllers
{
    public class ManagedController : Controller
    {
        private readonly HelperFileAzure helperFile;
        private readonly ServiceApiUsers serviceUsers;
        private readonly ServiceLogicApps serviceLogicApps;

        public ManagedController(HelperFileAzure helperFile, ServiceLogicApps serviceLogicApps, ServiceApiUsers serviceUsers)
        {
            this.serviceUsers = serviceUsers;
            this.helperFile = helperFile;
            this.serviceLogicApps = serviceLogicApps;
        }

        public IActionResult AccessError()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usernameOrEmail, string password)
        {
            // Pass to findUser the userId
            var tokenUser = await this.serviceUsers.LoginUserAsync(usernameOrEmail, password);

            AppUser? user = tokenUser?.Item2;

            if (user == null)
            {
                ViewData["MESSAGE"] = "Usuario/password incorrectos";
                return View();
            }
            else if (user.Approved == false)
            {
                string token = await this.serviceUsers.RequestChangeDataAsync();
                string resendUrl = Url.Action("ResendConfirmationEmail", "Managed", new { userId = user.Id, token });
                ViewData["MESSAGE"] = $"Este usuario no ha sido validado, <a class='text-blue-700 hover:underline dark:text-blue-500' href='{resendUrl}'>Quiero recibir de nuevo la confirmación por correo</a>";
                return View();
            }

            HttpContext.Session.SetString("TOKEN", tokenUser.Item1);

            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

            Claim claimName = new(ClaimTypes.Name, user.UserName);
            identity.AddClaim(claimName);

            Claim claimEmail = new(ClaimTypes.Email, user.Email);
            identity.AddClaim(claimEmail);

            Claim claimId = new(ClaimTypes.NameIdentifier, user.Id.ToString());
            identity.AddClaim(claimId);

            Claim claimRole = new(ClaimTypes.Role, user.Role);
            identity.AddClaim(claimRole);

            string imageUrl = await this.helperFile.GetBlobUriAsync(Containers.ProfileImages, user.Image);
            Claim claimImage = new("IMAGE", imageUrl);
            identity.AddClaim(claimImage);

            Claim claimToken = new("TOKEN", tokenUser.Item1);
            identity.AddClaim(claimToken);

            ClaimsPrincipal userPrincipal = new(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

            string controller = TempData["controller"].ToString();
            string action = TempData["action"].ToString();

            return RedirectToAction(action, controller);
        }

        public async Task<IActionResult> ResendConfirmationEmail(int userId, string token)
        {
            AppUser? user = await this.serviceUsers.FindUserAsync(userId);

            if (user != null)
            {
                // Confirmation mail
                string protocol = HttpContext.Request.IsHttps ? "https" : "http";
                string domainName = HttpContext.Request.Host.Value.ToString();
                string baseUrl = protocol + domainName;
                string url = Url.Action("ApproveUserEmail", "Users", new { userId, token }, protocol)!;

                List<MailLink> links = new()
            {
                new MailLink()
                {
                    LinkText = "Confirmar cuenta",
                    Link = url
                }
            };
                await this.serviceLogicApps.SendMailAsync(user.Email, "Confirmación de cuenta", "Se ha solicitado una petición para crear una cuenta en MoodReboot con este correo electrónico. Pulsa el siguiente enlace para confirmarla. Si no has sido tu el solicitante no te procupes, la petición será cancelada en un período de 24hrs.", links, baseUrl);

                ViewData["SUCCESS"] = "Correo de confirmación enviado";
                return View("Login");
            }
            ViewData["MESSAGE"] = "Datos inválidos";
            return View("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignUp(bool noUserImage)
        {
            ViewData["NO_IMAGE"] = noUserImage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp
            (string userName, string firstName, string lastName, string email, string password, IFormFile image)
        {
            string fileName = "";

            // Upload profile image
            if (image != null)
            {
                int maximo = await this.serviceUsers.GetMaxUserAsync();

                fileName = "image_" + maximo;

                bool isUploaded = await this.helperFile.UploadFileAsync(image, Containers.ProfileImages, FileTypes.Image, fileName);

                if (isUploaded == false)
                {
                    ViewData["ERROR"] = "Error al subir archivo";
                    return View();
                }
            }

            // BBDD
            int userId = await this.serviceUsers.RegisterUserAsync(userName, firstName, lastName, email, password, fileName);

            // Confirmation token
            string token = await this.serviceUsers.RequestChangeDataAsync(userId);

            // Confirmation mail
            string protocol = HttpContext.Request.IsHttps ? "https" : "http";
            string domainName = HttpContext.Request.Host.Value.ToString();
            string baseUrl = protocol + domainName;
            string url = Url.Action("ApproveUserEmail", "Users", new { userId, token }, protocol)!;

            List<MailLink> links = new()
            {
                new MailLink()
                {
                    LinkText = "Confirmar cuenta",
                    Link = url
                }
            };
            await this.serviceLogicApps.SendMailAsync(email, "Confirmación de cuenta", "Se ha solicitado una petición para crear una cuenta en MoodReboot con este correo electrónico. Pulsa el siguiente enlace para confirmarla. Si no has sido tu el solicitante no te procupes, la petición será cancelada en un período de 24hrs.", links, baseUrl);

            ViewData["SUCCESS"] = "Revisa tu correo electrónico";
            return View();
        }

        // Forms validations
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> EmailExists(string email)
        {
            if (await this.serviceUsers.IsEmailAvailableAsync(email) == false)
            {
                return Json(true);
            }
            return Json($"Email {email} no pertenece a ningún usuario de la plataforma.");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (await this.serviceUsers.IsEmailAvailableAsync(email) == false)
            {
                return Json($"Email {email} ya está en uso.");
            }

            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyUsername(string userName)
        {
            if (await this.serviceUsers.IsUsernameAvailableAsync(userName) == false)
            {
                return Json($"Nick {userName} ya está en uso.");
            }

            return Json(true);
        }
    }
}
