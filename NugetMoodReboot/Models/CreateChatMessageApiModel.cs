namespace NugetMoodReboot.Models
{
    public class CreateChatMessageApiModel
    {
        public int GroupChatId { get; set; }
        public string UserName { get; set; }
        public string? Text { get; set; }
        public int? FileId { get; set; }
    }
}
