namespace NugetMoodReboot.Models
{
    public class CourseEnrollmentApiModel
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string? Password { get; set; }
        public bool IsEditor { get; set; }
    }
}
