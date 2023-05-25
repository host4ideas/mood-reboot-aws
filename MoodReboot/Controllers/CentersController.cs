using Microsoft.AspNetCore.Mvc;
using MoodReboot.Helpers;
using MoodReboot.Models;
using MoodReboot.Services;
using MvcCoreSeguridadEmpleados.Filters;
using MvcLogicApps.Services;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;
using System.ComponentModel;
using System.Security.Claims;

namespace MoodReboot.Controllers
{
    public class CentersController : Controller
    {
        private readonly ServiceApiCenters serviceCenters;
        private readonly ServiceApiCourses serviceCourses;
        private readonly ServiceLogicApps serviceLogicApps;
        private readonly HelperFileAzure helperFile;

        public CentersController(ServiceApiCourses serviceCourses, ServiceApiCenters serviceCenters, ServiceLogicApps serviceLogicApps, HelperFileAzure helperFile)
        {
            this.serviceCenters = serviceCenters;
            this.serviceCourses = serviceCourses;
            this.serviceLogicApps = serviceLogicApps;
            this.helperFile = helperFile;
        }

        public async Task<IActionResult> Index()
        {
            List<CenterListView> centers = await this.serviceCenters.GetAllCentersAsync();
            return View(centers);
        }

        public async Task<IActionResult> CenterDetails(int id)
        {
            Center? center = await this.serviceCenters.FindCenterAsync(id);
            bool isEditor = false;

            if (HttpContext.User.Identity.IsAuthenticated == true)
            {
                int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                List<AppUser> users = await this.serviceCenters.GetCenterEditorsAsync(id);

                foreach (AppUser user in users)
                {
                    if (user.Id == userId)
                    {
                        isEditor = true;
                    }
                }
            }

            if (center == null)
            {
                return RedirectToAction("UserCenters", "Centers");
            }

            List<CourseListView> courses = await this.serviceCourses.CenterCoursesListViewAsync(id);

            ViewData["IS_EDITOR"] = isEditor;
            ViewData["CENTER"] = center;
            return View(courses);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> UserCenters()
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<CenterListView> centers = await this.serviceCenters.GetUserCentersAsync(userId);
            return View("Index", centers);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> RemoveUserCenter(int userId, int centerId)
        {
            await this.serviceCenters.RemoveUserCenterAsync(userId, centerId);
            return RedirectToAction("DirectorView", new { centerId });
        }

        [HttpPost]
        public async Task<IActionResult> AddCenterEditors(int centerId, List<int> userIds)
        {
            await this.serviceCenters.AddEditorsCenterAsync(centerId, userIds);
            return RedirectToAction("DirectorView", new { centerId });
        }

        #region EDITOR VIEW

        [AuthorizeUsers]
        public async Task<IActionResult> EditorView(int centerId)
        {
            Center? center = await this.serviceCenters.FindCenterAsync(centerId);
            if (center == null)
            {
                return RedirectToAction("UserCenters", "Centers");
            }

            // Center editor courses where it's editor
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["COURSES"] = await this.serviceCourses.GetEditorCenterCoursesAsync(userId, centerId);

            ViewData["CENTER"] = center;
            return View(new CreateCourseModel());
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> EditorView(int centerId, string name, bool isVisible, string description, IFormFile image, string password)
        {
            string fileName = "course_image_" + await this.serviceCourses.GetMaxCourseAsync();

            bool uploaded = await this.helperFile.UploadFileAsync(image, Containers.PrivateContent, FileTypes.Image, fileName);

            if (!uploaded)
            {
                ViewData["ERROR"] = "Error al subir el archivo";
            }
            else
            {
                bool result = await this.serviceCenters.CreateCourseAsync(centerId, name, isVisible, fileName, description, password);

                if (!result)
                {
                    ViewData["ERROR"] = "Error al crear el curso";
                }
            }

            return RedirectToAction("EditorView");
        }

        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            await this.serviceCourses.DeleteCourseAsync(courseId);
            return RedirectToAction("EditorView");
        }

        public async Task<IActionResult> CourseVisibility(int courseId)
        {
            await this.serviceCourses.UpdateCourseVisibilityAsync(courseId);
            return RedirectToAction("EditorView");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyCenter(int centerId)
        {
            Center? center = await this.serviceCenters.FindCenterAsync(centerId);
            if (center == null)
            {
                return Json("El centro no existe");
            }

            return Json(true);
        }

        #endregion

        #region DIRECTOR VIEW

        [AuthorizeUsers]
        public async Task<IActionResult> DirectorView(int centerId)
        {
            Center? center = await this.serviceCenters.FindCenterAsync(centerId);
            if (center == null)
            {
                return RedirectToAction("UserCenters", "Centers");
            }

            List<AppUser> users = await this.serviceCenters.GetCenterEditorsAsync(centerId);
            // Remove the current user from the list
            users.RemoveAll(x => x.Id == int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
            List<CourseListView> courses = await this.serviceCourses.GetCenterCoursesAsync(centerId);
            ViewData["COURSES"] = courses;
            ViewData["CENTER"] = center;
            return View(users);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> DirectorView(int centerId, string centerEmail, string centerName, string centerAddress, string centerTelephone, IFormFile centerImage)
        {
            string fileName = "center_image_" + centerId;

            bool isUploaded = await this.helperFile.UploadFileAsync(centerImage, Containers.PrivateContent, FileTypes.Image, fileName);

            if (isUploaded == false)
            {
                ViewData["ERROR"] = "Error al subir el archivo";
                return RedirectToAction("DirectorView", new { centerId });
            }
            await this.serviceCenters.UpdateCenterAsync(centerId, centerEmail, centerName, centerAddress, centerTelephone, fileName);
            return RedirectToAction("DirectorView", new { centerId });
        }

        #endregion

        #region CREATE CENTER

        [AuthorizeUsers]
        public IActionResult CenterRequest()
        {
            return View();
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> CenterRequest(string email, string centerEmail, string centerName, string centerAddress, string centerTelephone, IFormFile centerImage)
        {
            int director = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            int maximo = await this.serviceCenters.GetMaxCenterAsync();

            string fileName = "center_image_" + maximo;

            bool isUploaded = await this.helperFile.UploadFileAsync(centerImage, Containers.PrivateContent, FileTypes.Image, fileName);

            if (isUploaded == false)
            {
                ViewData["ERROR"] = "Error al subir archivo";
                return View();
            }

            await this.serviceCenters.CreateCenterAsync(centerEmail, centerName, centerAddress, centerTelephone, fileName, director, false);
            string protocol = HttpContext.Request.IsHttps ? "https" : "http";
            string domainName = HttpContext.Request.Host.Value.ToString();
            string baseUrl = protocol + domainName;
            await this.serviceLogicApps.SendMailAsync(email, "Aprobación de centro en curso", "Estamos en proceso de aprobar su solicitud de creación de centro. Por favor, si ha cometido algún error en los datos o quisiera cancelar la operación. Mande un correo a: moodreboot@gmail.com", baseUrl);
            ViewData["MESSAGE"] = "Solicitud enviada";
            return View();
        }

        #endregion
    }
}
