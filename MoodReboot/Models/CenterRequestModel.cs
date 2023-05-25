using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MoodReboot.Models
{
    public class CenterRequestModel
    {
        [Display(Name = "Email del centro")]
        [Required(ErrorMessage = "Email requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string CenterEmail { get; set; }
        [Display(Name = "Email del director")]
        [Required(ErrorMessage = "Email requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Remote(action: "EmailExists", controller: "Managed")]
        public string Email { get; set; }
        [Display(Name = "Nombre del centro")]
        [Required(ErrorMessage = "Nombre del centro requerido")]
        [StringLength(50, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 10)]
        public string CenterName { get; set; }
        [Display(Name = "Dirección del centro")]
        [Required(ErrorMessage = "Dirección requerida")]
        [StringLength(150, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 10)]
        public string CenterAddress { get; set; }
        [Display(Name = "Teléfono del centro")]
        [Required(ErrorMessage = "Teléfono requerido")]
        [Phone(ErrorMessage = "Teléfono inválido")]
        public string CenterTelephone { get; set; }
        [Display(Name = "Imagen del centro")]
        [Required(ErrorMessage = "Imagen del centro requerida")]
        public IFormFile CenterImage { get; set; }
    }
}
