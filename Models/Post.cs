using System.ComponentModel.DataAnnotations;
using TechBlog.Data.Enums;

namespace TechBlog.Models
{
    public class Post
    {
        public int PostId { get; set; }
        [Required(ErrorMessage ="El título es requerido")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 100 caracteres")]
        public string? Titulo { get; set; }
        [Required(ErrorMessage = "El contenido es requerido")]
        [StringLength(5000, MinimumLength = 50, ErrorMessage = "El contenido debe tener entre 50 y 5000 caracteres")]
        public string? Contenido {get; set;}
        [Required(ErrorMessage = "La categoría es requerida")]
        public CategoriaEnum? Categoria {  get; set;} // enumeracion
        //[Required(ErrorMessage = "La fecha de creacion es requerida")]
        public DateTime FechaCreacion {  get; set;}
    }
}
