using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("COURSE")]
    public class Course
    {
        [Key]
        [Column("COURSE_ID")]
        public int Id { get; set; }
        [Column("DATE_PUBLISHED")]
        public DateTime DatePublished { get; set; }
        [Column("DATE_MODIFIED")]
        public DateTime? DateModified { get; set; }
        [Column("DESCRIPTION")]
        public string? Description { get; set; }
        [Column("IMAGE")]
        public string? Image { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("CENTER_ID")]
        public int CenterId { get; set; }
        [Column("IS_VISIBLE")]
        public bool IsVisible { get; set; }
        [Column("GROUP_ID")]
        public int? GroupId { get; set; }
        [Column("PASSWORD")]
        public string? Password { get; set; }
    }
}
