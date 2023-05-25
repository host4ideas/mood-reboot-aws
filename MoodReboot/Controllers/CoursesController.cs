using Microsoft.AspNetCore.Mvc;
using MoodReboot.Extensions;
using MoodReboot.Helpers;
using MoodReboot.Services;
using MvcCoreSeguridadEmpleados.Filters;
using NugetMoodReboot.Models;
using System.Security.Claims;

namespace MoodReboot.Controllers
{
    [AuthorizeUsers]
    public class CoursesController : Controller
    {
        private readonly ServiceApiCourses serviceCourses;
        private readonly ServiceApiContents serviceContents;
        private readonly ServiceApiContentGroups serviceCtnGroups;
        private readonly ServiceApiUsers serviceUsers;
        private readonly HelperFileAzure helperFileAzure;

        public CoursesController(ServiceApiCourses serviceCourses, ServiceApiContents serviceContents, ServiceApiContentGroups serviceCtnGroups, ServiceApiUsers serviceUsers, HelperFileAzure helperFileAzure)
        {
            this.serviceCourses = serviceCourses;
            this.serviceContents = serviceContents;
            this.serviceCtnGroups = serviceCtnGroups;
            this.serviceUsers = serviceUsers;
            this.helperFileAzure = helperFileAzure;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserCourses()
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<CourseListView> courses = await this.serviceCourses.GetUserCoursesAsync(userId);
            return View("Index", courses);
        }

        public async Task<IActionResult> DeleteCourseUser(int courseId, int userId)
        {
            Course? course = await this.serviceCourses.FindCourseAsync(courseId);
            if (course != null)
            {
                await this.serviceCourses.RemoveCourseUserAsync(courseId, userId);
                if (course.GroupId.HasValue)
                {
                    await this.serviceUsers.RemoveChatUserAsync(userId, course.GroupId.Value);
                }
            }
            return RedirectToAction("CourseDetails", new { id = courseId });
        }

        public async Task<IActionResult> DeleteCourseEditor(int courseId, int userId)
        {
            await this.serviceCourses.RemoveCourseEditorAsync(courseId, userId);
            return RedirectToAction("CourseDetails", new { id = courseId });
        }

        public async Task<IActionResult> AddCourseEditor(int courseId, int userId)
        {
            await this.serviceCourses.AddCourseEditorAsync(courseId, userId);
            return RedirectToAction("CourseDetails", new { id = courseId });
        }

        public async Task<IActionResult> UpdateCourse(int userId, int courseId, string description, string image, string name, bool isVisible)
        {
            await this.serviceCourses.UpdateCourseAsync(courseId, description, image, name, isVisible);
            return RedirectToAction("UserCourses", new { id = userId });
        }

        public async Task<IActionResult> CenterCourses(int centerId)
        {
            List<CourseListView> courses = await this.serviceCourses.GetCenterCoursesAsync(centerId);
            return View("Index", courses);
        }

        public async Task<IActionResult> GetAllCourses()
        {
            List<Course> courses = await this.serviceCourses.GetAllCoursesAsync();
            return View("Index", courses);
        }

        public async Task<IActionResult> CourseEnrollment(int courseId)
        {
            Course? course = await this.serviceCourses.FindCourseAsync(courseId);

            // The course doesn't exist
            if (course == null)
            {
                return RedirectToAction("UserCourses");
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseEnrollment(int courseId, int userId, string password, bool isEditor = false)
        {
            Course? course = await this.serviceCourses.FindCourseAsync(courseId);

            if (course != null)
            {
                // If the course doesn't have password, enroll the user in the course
                if (course.Password == null)
                {
                    await this.serviceCourses.AddCourseUserAsync(courseId, userId, isEditor);
                    return RedirectToAction("CourseDetails", new { courseId });
                }
                // If the course has password
                bool added = await this.serviceCourses.AddCourseUserAsync(courseId, userId, isEditor, password);
                if (added == true)
                {
                    ViewData["ERROR"] = "Contraseña del curso incorrecta";
                    return RedirectToAction("CourseEnrollment", new { courseId });
                }
            }
            // Fallback to user's courses
            return RedirectToAction("UserCourses", new { id = courseId });
        }

        [AuthorizeUsers]
        public async Task<IActionResult> CourseDetails(int courseId)
        {
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            UserCourse? userCourse = await this.serviceCourses.FindUserCourseAsync(userId, courseId);

            if (userCourse != null)
            {
                List<ContentGroup> contentGroups = await this.serviceCtnGroups.GetCourseContentGroupsAsync(courseId);

                foreach (ContentGroup group in contentGroups)
                {
                    List<Content> contentList = await this.serviceContents.GetContentByGroupAsync(group.ContentGroupId);
                    List<ContentListModel> contentFileList = new();

                    foreach (Content ctn in contentList)
                    {
                        ContentListModel contentFile = new()
                        {
                            ContentGroupId = ctn.ContentGroupId,
                            Id = ctn.Id,
                            Text = ctn.Text,
                            FileId = ctn.FileId
                        };

                        if (ctn.FileId != null)
                        {
                            contentFile.File = await this.serviceUsers.FindFileAsync(ctn.FileId.Value);

                            // Transform the name of the file to the URI of it
                            contentFile.File.Name = await this.helperFileAzure.GetBlobUriAsync(Containers.PrivateContent, contentFile.File.Name);
                        }

                        contentFileList.Add(contentFile);
                    }

                    group.Contents = contentFileList;
                }

                Course? course = await this.serviceCourses.FindCourseAsync(courseId);

                if (course == null || contentGroups == null)
                {
                    return RedirectToAction("UserCourses", new { userId });
                }

                // Add to last seen courses
                List<LastSeenCourse>? lastSeenCourses = HttpContext.Session.GetObject<List<LastSeenCourse>>("LAST_COURSES");

                if (lastSeenCourses == null)
                {
                    lastSeenCourses = new();
                }
                else if (lastSeenCourses.Count == 5)
                {
                    lastSeenCourses.RemoveAt(0);
                }

                lastSeenCourses.Add(new LastSeenCourse()
                {
                    Id = course.Id,
                    Description = course.Description,
                    Image = course.Image,
                    Name = course.Name,
                });

                // Save without duplicates
                HttpContext.Session.SetObject("LAST_COURSES", lastSeenCourses.DistinctBy(x => x.Id).ToList());

                // Course users
                List<CourseUsersModel> courseUsers = await this.serviceCourses.GetCourseUsersAsync(course.Id);

                CourseDetailsModel details;

                if (userCourse.IsEditor)
                {
                    details = new()
                    {
                        ContentGroups = contentGroups,
                        Course = course,
                        CourseUsers = courseUsers,
                        IsEditor = true
                    };
                }
                else
                {
                    details = new()
                    {
                        ContentGroups = contentGroups,
                        Course = course,
                        IsEditor = false
                    };
                }

                return View(details);
            }
            // If the user is not already enrolled to the course
            return RedirectToAction("CourseEnrollment", new { courseId });
        }

        public IActionResult DeleteCourse(int id)
        {
            this.serviceCourses.DeleteCourseAsync(id);
            return RedirectToAction("UserCourses");
        }
    }
}
