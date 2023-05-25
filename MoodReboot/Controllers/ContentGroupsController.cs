using Microsoft.AspNetCore.Mvc;
using MoodReboot.Services;

namespace MoodReboot.Controllers
{
    public class ContentGroupsController : Controller
    {
        private readonly ServiceApiContentGroups serviceCtnGroups;

        public ContentGroupsController(ServiceApiContentGroups serviceCtnGroups)
        {
            this.serviceCtnGroups = serviceCtnGroups;
        }

        public async Task<IActionResult> DeleteContentGroup(int id, int courseId)
        {
            await this.serviceCtnGroups.DeleteContentGroupAsync(id);
            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateContentGroup(string name, int courseId, bool isVisible)
        {
            if (name != null && courseId >= 0)
            {
                await this.serviceCtnGroups.CreateContentGroupAsync(name, courseId, isVisible);
            }
            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContentGroup(int id, string name, int courseId, bool isVisible)
        {
            if (name != null && courseId >= 0)
            {
                await this.serviceCtnGroups.UpdateContentGroupAsync(id, name, isVisible);
            }
            return RedirectToAction("CourseDetails", "Courses", new { id = courseId });
        }
    }
}
