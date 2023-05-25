using Microsoft.AspNetCore.Http;

namespace NugetMoodReboot.Models
{
    public class CreateContentModel
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string UnsafeHtml { get; set; }
        public IFormFile? File { get; set; }
    }
}
