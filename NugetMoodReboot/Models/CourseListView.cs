namespace NugetMoodReboot.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
    }

    public class CourseListView
    {
        public string Description { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime? DateModified { get; set; }
        public string CourseName { get; set; }
        public bool IsEditor { get; set; }
        public string CenterName { get; set; }
        public int CourseId { get; set; }
        public string? Image { get; set; }
        public bool IsVisible { get; set; }
        public List<Author>? Authors { get; set; }
    }
}
