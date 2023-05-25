namespace NugetMoodReboot.Models
{
    public class UpdateCourseApiModel
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public string? Password { get; set; }
    }
}
