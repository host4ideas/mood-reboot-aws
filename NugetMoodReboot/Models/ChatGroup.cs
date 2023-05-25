using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NugetMoodReboot.Models
{
    [Table("CHAT_GROUP")]
    public class ChatGroup
    {
        [Key]
        [Column("GROUP_ID")]
        public int Id { get; set; }
        [Column("IMAGE")]
        public string? Image { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
    }
}
