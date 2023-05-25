using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MoodReboot.Models
{
    public class CreateCourseModel
    {
        [Required(ErrorMessage = "Error al procesar la petición")]
        [Remote(action: "VerifyCenter", controller: "Centers")]
        public int CenterId { get; set; }
        [DisplayName("Descripción")]
        [StringLength(350, ErrorMessage = "{0}: tamaño máximo es de {1}.")]
        public string? Description { get; set; }
        [DisplayName("Imagen del curso")]
        [Required(ErrorMessage = "Imagen obligatoria")]
        [FileExtensions(ErrorMessage = "{0} formatos soportados: jpeg, jpg, png y webp", Extensions = "jpg,jpeg,png,webp")]
        public IFormFile? Image { get; set; }
        [DisplayName("Nombre del curso")]
        [Required(ErrorMessage = "Nombre del curso es obligatorio")]
        [StringLength(50, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 6)]
        public string Name { get; set; }
        [DisplayName("Marcar como visible")]
        public string IsVisible { get; set; }
        [DisplayName("Contraseña de matriculación")]
        [StringLength(12, ErrorMessage = "{0} debe de estar entre {2} y {1}.", MinimumLength = 4)]
        public string? Password { get; set; }
    }
}
