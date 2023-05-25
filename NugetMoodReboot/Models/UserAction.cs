using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("USER_ACTION")]
    public class UserAction
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("TOKEN")]
        public string Token { get; set; }
        [Column("REQUEST_DATE")]
        public DateTime RequestDate { get; set; }
    }
}
