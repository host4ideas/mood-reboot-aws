namespace NugetMoodReboot.Models
{
    public class UpdateFileApiModel
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public int? UserId { get; set; }
    }
}
