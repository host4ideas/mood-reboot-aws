using Microsoft.AspNetCore.Mvc;
using NugetMoodReboot.Models;
using Microsoft.AspNetCore.Authorization;
using NugetMoodReboot.Interfaces;

namespace APIMoodReboot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IRepositoryCenters repositoryCenters;
        private readonly IRepositoryCourses repositoryCourses;
        private readonly IRepositoryUsers repositoryUsers;

        public AdminController(IRepositoryCenters repositoryCenters, IRepositoryCourses repositoryCourses, IRepositoryUsers repositoryUsers)
        {
            this.repositoryCenters = repositoryCenters;
            this.repositoryCourses = repositoryCourses;
            this.repositoryUsers = repositoryUsers;
        }

        [HttpGet]
        public async Task<ActionResult> CenterRequests()
        {
            List<Center> centers = await this.repositoryCenters.GetPendingCentersAsync();
            return Ok(centers);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            await this.repositoryUsers.DeleteUserAsync(userId);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> UserRequests()
        {
            List<AppUser> users = await this.repositoryUsers.GetPendingUsersAsync();
            return Ok(users);
        }

        [HttpPut("{centerId}")]
        public async Task<ActionResult> ApproveCenter(int centerId)
        {
            Center? center = await this.repositoryCenters.FindCenterAsync(centerId);
            if (center != null)
            {
                await this.repositoryCenters.ApproveCenterAsync(center);
            }
            return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> ApproveUser(int userId)
        {
            AppUser? user = await this.repositoryUsers.FindUserAsync(userId);
            if (user != null)
            {
                await this.repositoryUsers.ApproveUserAsync(user);
                return NoContent();
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<ActionResult> Users()
        {
            List<AppUser> users = await this.repositoryUsers.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet]
        public async Task<ActionResult> Centers()
        {
            List<CenterListView> centers = await this.repositoryCenters.GetAllCentersAsync();
            return Ok(centers);
        }

        [HttpDelete("{centerId}")]
        public async Task<ActionResult> DeleteCenter(int centerId)
        {
            await this.repositoryCenters.DeleteCenterAsync(centerId);
            return RedirectToAction("Centers");
        }

        [HttpGet]
        public async Task<ActionResult> Courses()
        {
            List<Course> courses = await this.repositoryCourses.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpDelete("{courseId}")]
        public async Task<ActionResult> DeleteCourse(int courseId)
        {
            await this.repositoryCourses.DeleteCourseAsync(courseId);
            return NoContent();
        }

        [HttpPut("{courseId}")]
        public async Task<ActionResult> CourseVisibility(int courseId)
        {
            await this.repositoryCourses.UpdateCourseVisibilityAsync(courseId);
            return NoContent();
        }
    }
}
