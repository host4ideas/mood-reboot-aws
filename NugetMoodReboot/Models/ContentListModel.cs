using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    public class ContentListModel
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public int ContentGroupId { get; set; }
        public int? FileId { get; set; }
        public AppFile? File { get; set; }
    }
}
