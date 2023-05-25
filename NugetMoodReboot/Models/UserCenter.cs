using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("USER_CENTER")]
    public class UserCenter
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("CENTER_ID")]
        public int CenterId { get; set; }
        [Column("IS_EDITOR")]
        public bool IsEditor { get; set; }
    }
}
