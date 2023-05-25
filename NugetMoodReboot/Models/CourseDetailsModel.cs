namespace NugetMoodReboot.Models
{
    public class CourseDetailsModel
    {
        public Course Course { get; set; }
        public List<ContentGroup> ContentGroups { get; set; }
        public bool IsEditor { get; set; }
        // Optional: only if it's editor
        public List<CourseUsersModel>? CourseUsers { get; set; }
    }
}
