using Microsoft.AspNetCore.Mvc;
using MoodReboot.Services;
using MvcCoreSeguridadEmpleados.Filters;
using MvcLogicApps.Services;
using NugetMoodReboot.Helpers;
using NugetMoodReboot.Models;

namespace MoodReboot.Controllers
{
    [AuthorizeUsers(Policy = "ADMIN_ONLY")]
    public class AdminController : Controller
    {
        private readonly ServiceApiUsers serviceUsers;
        private readonly ServiceApiCenters serviceCenters;
        private readonly ServiceApiCourses serviceCourses;
        private readonly ServiceLogicApps serviceLogicApps;

        public AdminController(ServiceApiUsers serviceUsers, ServiceApiCenters serviceCenters, ServiceApiCourses serviceCourses, ServiceLogicApps serviceLogicApps)
        {
            this.serviceUsers = serviceUsers;
            this.serviceCenters = serviceCenters;
            this.serviceCourses = serviceCourses;
            this.serviceLogicApps = serviceLogicApps;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Requests()
        {
            return View();
        }

        public async Task<IActionResult> CenterRequests()
        {
            List<Center> centers = await this.serviceCenters.GetPendingCentersAsync();
            return PartialView("_PendingCentersPartial", centers);
        }

        public async Task<IActionResult> UserRequests()
        {
            List<AppUser> users = await this.serviceUsers.GetPendingUsersAsync();
            return PartialView("_PendingUsersPartial", users);
        }

        public async Task<IActionResult> ApproveCenter(int centerId)
        {
            Center? center = await this.serviceCenters.FindCenterAsync(centerId);
            if (center != null)
            {
                await this.serviceCenters.ApproveCenterAsync(center);

                string protocol = HttpContext.Request.IsHttps ? "https" : "http";
                string domainName = HttpContext.Request.Host.Value.ToString();
                string baseUrl = protocol + domainName;
                await this.serviceLogicApps.SendMailAsync(center.Email, "Centro aprobado", "Tu centro ha sido aprobado en la plataforma MoodReboot, puedes iniciar sesión en tu perfil y empezar a administrarlo", baseUrl);
            }
            return RedirectToAction("Requests");
        }

        public async Task<IActionResult> ApproveUser(int userId)
        {
            AppUser? user = await this.serviceUsers.FindUserAsync(userId);
            if (user != null)
            {
                await this.serviceUsers.ApproveUserAsync(user.Id);
                string protocol = HttpContext.Request.IsHttps ? "https" : "http";
                string domainName = HttpContext.Request.Host.Value.ToString();
                string baseUrl = protocol + domainName;
                await this.serviceLogicApps.SendMailAsync(user.Email, "Usuario aprobado", "Tu cuenta en MoodReboot ha sido activada, por favor, inicia sesión con tu cuenta para empezar a utilizar nuestra plataforma.", baseUrl);
            }
            return RedirectToAction("Requests");
        }

        public async Task<IActionResult> Users()
        {
            List<AppUser> users = await this.serviceUsers.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> DeleteUser(int userId)
        {
            await this.serviceUsers.DeleteUserAsync(userId);
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Centers()
        {
            List<CenterListView> centers = await this.serviceCenters.GetAllCentersAsync();
            return View(centers);
        }

        public async Task<IActionResult> DeleteCenter(int centerId)
        {
            await this.serviceCenters.DeleteCenterAsync(centerId);
            return RedirectToAction("Centers");
        }

        public async Task<IActionResult> Courses()
        {
            List<Course> courses = await this.serviceCourses.GetAllCoursesAsync();
            return View(courses);
        }

        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            await this.serviceCourses.DeleteCourseAsync(courseId);
            return RedirectToAction("Courses");
        }

        public async Task<IActionResult> CourseVisibility(int courseId)
        {
            await this.serviceCourses.UpdateCourseVisibilityAsync(courseId);
            return RedirectToAction("Courses");
        }
    }
}
