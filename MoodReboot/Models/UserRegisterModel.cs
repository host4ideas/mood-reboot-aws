using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoodReboot.Models
{
    public class UserRegisterModel
    {
        // Validations
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Remote(action: "VerifyEmail", controller: "Managed")]
        public string Email { get; set; }
        // Validations
        [Display(Name = "Confirmar email")]
        [Compare("Email", ErrorMessage = "Los emails no coinciden")]
        public string? ConfirmEmail { get; set; }
        // Validations
        [PasswordPropertyText]
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Contraseña requerida")]
        [StringLength(20, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 6)]
        public byte[] Password { get; set; }
        // Validations
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [StringLength(20, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
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
        [Display(Name = "Imagen del usuario")]
        public IFormFile Image { get; set; }
    }
}
