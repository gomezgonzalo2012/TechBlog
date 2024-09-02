
using System.ComponentModel.DataAnnotations;

namespace TechBlog.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        [Required(ErrorMessage ="El campo nombre es obligatorio")]
        [StringLength(500, ErrorMessage ="El campo nombre no debe superar los {1} caracteres")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El campo apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo apellido no debe superar los {1} caracteres")]
        public string? Apellido {  get; set; }
        [Required(ErrorMessage = "El campo correo es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo correo no debe superar los {1} caracteres")]
        public string? Correo {  get; set; }
        [Required(ErrorMessage = "El campo contraseña es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo contraseña no debe superar los {1} caracteres")]
        public string? Contrasenia { get; set; }
        public int RolId {  get; set; }
        public Rol? Rol { get; set; }
        [Required(ErrorMessage = "El campo nombre de usuario es obligatorio")]
        [StringLength(100, ErrorMessage = "El campo nombre de usuario no debe superar los {1} caracteres")]
        public string? NombreUsuario {  get; set; }
        public bool Estado {  get; set; }
        public string? Token {  get; set; }
        public DateTime FechaExpiracion { get; set; }

	
    }
}