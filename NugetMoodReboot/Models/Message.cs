using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("CHAT_MESSAGE")]
    public class Message
    {
        [Key]
        [Column("MESSAGE_ID")]
        public int MessageId { get; set; }
        [Column("GROUP_ID")]
        public int GroupId { get; set; }
        [Column("TEXT")]
        public string? Text { get; set; }
        [Column("USER_ID")]
        public int UserID { get; set; }
        [Column("DATE_POSTED")]
        public DateTime DatePosted { get; set; }
        [Column("FILE_ID")]
        public int? FileId { get; set; }
        [Column("USERNAME")]
        public string UserName { get; set; }
    }
}
