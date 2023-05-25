using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("APP_FILE")]
    public class AppFile
    {
        [Key]
        [Column("FILE_ID")]
        public int Id { get; set; }
        [Column("USER_ID")]
        public int? UserId { get; set; }
        [Column("MIME_TYPE")]
        public string MimeType { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
    }
}
