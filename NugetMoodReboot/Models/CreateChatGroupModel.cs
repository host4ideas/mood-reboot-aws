namespace NugetMoodReboot.Models
{

    public class CreateChatGroupModel
    {
        private string _groupName = "NEW CHAT GROUP";

        public List<int> UserIds { get; set; }
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value; }
        }
    }
}
