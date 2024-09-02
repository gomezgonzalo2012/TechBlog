using System.ComponentModel.DataAnnotations;

namespace TechBlog.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage ="Por favor, introduce un correo electrónico válido.")]
        [StringLength(50, MinimumLength = 20, ErrorMessage = "El correo electrónico debe contener un máximo de {1} caracteres.")]
        public string? Correo { get; set; }
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [StringLength(50,MinimumLength =8, ErrorMessage ="La contraseña debe contener al menos {2} caracteres.")]
        [DataType(DataType.Password)]
        public string? Contrasenia { get; set; }
        [Display(Name ="Mantener sesión activa.")] // se mostrara en la vista
        public bool MantenerActivo { get; set; }
    }
}
