using NugetMoodReboot.Helpers;

namespace NugetMoodReboot.Models
{
    public class SendEmailModel
    {
        public string To { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string BaseUrl { get; set; }
        public List<MailLink>? Links { get; set; }
    }
}
