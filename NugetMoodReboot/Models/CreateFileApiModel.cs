namespace NugetMoodReboot.Models
{
    public class CreateFileApiModel
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public int? UserId { get; set; }
    }
}
