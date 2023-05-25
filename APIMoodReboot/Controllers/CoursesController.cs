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
    public class CoursesController : ControllerBase
    {
        private readonly IRepositoryCourses repositoryCourses;
        private readonly IRepositoryUsers repositoryUsers;

        public CoursesController(IRepositoryCourses repositoryCourses, IRepositoryUsers repositoryUsers)
        {
            this.repositoryCourses = repositoryCourses;
            this.repositoryUsers = repositoryUsers;
        }

        [HttpGet("{courseId}")]
        public async Task<ActionResult<UserCourse?>> FindUserCourse(int courseId)
        {
            Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            return await this.repositoryCourses.FindUserCourseAsync(user.Id, courseId);
        }

        [HttpGet]
        public async Task<ActionResult> UserCourses()
        {
                        Claim claim = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData");
            string jsonUser = claim.Value;
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonUser);

            List<CourseListView> courses = await this.repositoryCourses.GetUserCoursesAsync(user.Id);
            return Ok(courses);
        }

        [HttpGet("{courseId}")]
        public async Task<ActionResult<Course?>> FindCourse(int courseId)
        {
            return await this.repositoryCourses.FindCourseAsync(courseId);
        }

        [HttpDelete("{courseId}/{userId}")]
        public async Task<ActionResult> DeleteCourseUser(int courseId, int userId)
        {
            Course? course = await this.repositoryCourses.FindCourseAsync(courseId);
            if (course != null)
            {
                await this.repositoryCourses.RemoveCourseUserAsync(courseId, userId);
                if (course.GroupId.HasValue)
                {
                    await this.repositoryUsers.RemoveChatUserAsync(userId, course.GroupId.Value);
                }
            }
            return RedirectToAction("CourseDetails", new { id = courseId });
        }

        [HttpDelete("{courseId}/{userId}")]
        public async Task<ActionResult> DeleteCourseEditor(int courseId, int userId)
        {
            await this.repositoryCourses.RemoveCourseEditorAsync(courseId, userId);
            return NoContent();
        }

        [HttpPost("{courseId}/{userId}")]
        public async Task<ActionResult> AddCourseEditor(int courseId, int userId)
        {
            await this.repositoryCourses.AddCourseEditorAsync(courseId, userId);
            return CreatedAtAction(null, null);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCourseVisibility(int courseId)
        {
            await this.repositoryCourses.UpdateCourseVisibilityAsync(courseId);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCourse(UpdateCourseApiModel course)
        {
            await this.repositoryCourses.UpdateCourseAsync(course.Id, course.Description, course.Image, course.Name, course.IsVisible);
            return NoContent();
        }

        [HttpGet("{centerId}")]
        public async Task<ActionResult<List<CourseListView>>> CenterCourses(int centerId)
        {
            return await this.repositoryCourses.GetCenterCoursesAsync(centerId);
        }

        [HttpGet("{userId}/{centerId}")]
        public async Task<ActionResult<List<CourseListView>>> GetEditorCenterCourses(int userId, int centerId)
        {
            return await this.repositoryCourses.GetEditorCenterCoursesAsync(userId, centerId);
        }

        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetAllCourses()
        {
            return await this.repositoryCourses.GetAllCoursesAsync();
        }

        [HttpGet("{courseId}")]
        public async Task<ActionResult<List<CourseUsersModel>>> GetCourseUsers(int courseId)
        {
            return await this.repositoryCourses.GetCourseUsersAsync(courseId);
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetMaxCourse()
        {
            return await this.repositoryCourses.GetMaxCourseAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CourseEnrollment(CourseEnrollmentApiModel model)
        {
            Course? course = await this.repositoryCourses.FindCourseAsync(model.CourseId);

            if (course == null)
            {
                return NotFound();
            }

            // If the course doesn't have password, enroll the user in the course
            if (course.Password == null)
            {
                await this.repositoryCourses.AddCourseUserAsync(model.CourseId, model.UserId, model.IsEditor);
                return NoContent();
            }

            // If the course has password
            bool added = await this.repositoryCourses.AddCourseUserAsync(model.CourseId, model.UserId, model.IsEditor, model.Password);

            if (added == true)
            {
                return NoContent();
            }

            return Problem("Contraseña del curso incorrecta");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            await this.repositoryCourses.DeleteCourseAsync(id);
            return NoContent();
        }
    }
}
