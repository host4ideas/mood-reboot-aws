using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("USER_GROUP")]
    public class UserChatGroup
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("USER_ID")]
        public int UserID { get; set; }
        [Column("GROUP_ID")]
        public int GroupId { get; set; }
        [Column("JOIN_DATE")]
        public DateTime JoinDate { get; set; }
        [Column("LAST_SEEN")]
        public DateTime LastSeen { get; set; }
        [Column("IS_ADMIN")]
        public bool IsAdmin { get; set; }
    }
}
