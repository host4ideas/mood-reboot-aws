using APIMoodReboot.Data;
using NugetMoodReboot.Interfaces;
using NugetMoodReboot.Models;

namespace APIMoodReboot.Helpers
{
    public class HelperCourse
    {
        private readonly MoodRebootContext context;
        private readonly IRepositoryCourses repositoryCourses;
        private readonly IRepositoryUsers repositoryUsers;

        public HelperCourse(MoodRebootContext context, IRepositoryCourses repositoryCourses, IRepositoryUsers repositoryUsers)
        {
            this.context = context;
            this.repositoryCourses = repositoryCourses;
            this.repositoryUsers = repositoryUsers;
        }

        public async Task<bool> CreateCourse(int centerId, int firstEditorId, string name, bool isVisible, string? path, string? description, string? password)
        {
            // Chat group name max 40 characters
            string chatGroupName = "FORO " + name;
            // Create chat group
            int chatGroupId = await repositoryUsers.NewChatGroupAsync(new HashSet<int> { firstEditorId }, firstEditorId, chatGroupName);

            int newCourseId = await repositoryCourses.GetMaxCourseAsync();

            if (path == null)
            {
                path = "default_course_image.jpeg";
            }

            // Create the course
            await context.Courses.AddAsync(new Course()
            {
                CenterId = centerId,
                DateModified = DateTime.Now,
                DatePublished = DateTime.Now,
                Description = description,
                Image = path,
                Password = password,
                GroupId = chatGroupId,
                Id = newCourseId,
                IsVisible = isVisible,
                Name = name,
            });

            await context.SaveChangesAsync();
            return true;
        }
    }
}
