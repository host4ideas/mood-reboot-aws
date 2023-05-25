using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("CENTER")]
    public class Center
    {
        [Key]
        [Column("CENTER_ID")]
        public int Id { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("ADDRESS")]
        public string Address { get; set; }
        [Column("IMAGE")]
        public string? Image { get; set; }
        [Column("TELEPHONE")]
        public string Telephone { get; set; }
        [Column("DIRECTOR")]
        public int Director { get; set; }
        [Column("APPROVED")]
        public bool Approved { get; set; }
    }
}
