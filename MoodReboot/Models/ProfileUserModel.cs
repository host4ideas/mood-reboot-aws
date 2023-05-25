using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MoodReboot.Models
{
    public class ProfileUserModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Remote(action: "VerifyEmail", controller: "Managed")]
        public string Email { get; set; }
        // Validations
        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage = "Nombre de usuario requerido")]
        [StringLength(20, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 4)]
        [Remote(action: "VerifyUsername", controller: "Managed")]
        public string UserName { get; set; }
        // Validations
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Nombre requerido")]
        [StringLength(20, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 2)]
        public string FirstName { get; set; }
        // Validations
        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Apellidos requeridos")]
        [StringLength(30, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 2)]
        public string LastName { get; set; }
        // Validations
        public string? Image { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
