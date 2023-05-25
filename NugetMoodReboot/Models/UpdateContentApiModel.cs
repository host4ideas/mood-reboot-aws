namespace NugetMoodReboot.Models
{
    public class UpdateContentApiModel
    {
        public int ContentId { get; set; }

        public int UserId { get; set; }

        public string? UnsafeHtml { get; set; }

        public AppFile? File { get; set; }
    }
}
