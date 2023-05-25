using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("USER_COURSE")]
    public class UserCourse
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
        [Column("COURSE_ID")]
        public int CourseId { get; set; }
        [Column("IS_EDITOR")]
        public bool IsEditor { get; set; }
    }
}
