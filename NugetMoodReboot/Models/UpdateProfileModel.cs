using Microsoft.AspNetCore.Http;

namespace NugetMoodReboot.Models
{
    public class UpdateProfileModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile Image { get; set; }
    }
}
