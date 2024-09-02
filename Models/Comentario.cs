using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechBlog.Models
{
    public class Comentario
    {
        public int ComentarioId {  get; set; }
        [Required(ErrorMessage ="El contenido es requerido")]
        [StringLength(5000,MinimumLength =100, ErrorMessage ="El contenido debe tener una longitud entre {0} y {1}")]
        public string? Contenido {  get; set; }
	    public DateTime FechaCreacion {  get; set; }
	    public int UsuarioId {get; set; }
	    public int PostId {get; set; }
	    public int? ComentarioPadreId { get; set; } // solo un padre
        public int? ComentarioAbueloId { get; set; } // en caso de que this post tenga un padre y ese tenga un padre 
                                                     // osea este comentario sea nieto (3 niveles)
        public List<Comentario>? ComentariosHijos { get; set; } // lista de subcomentarios 
        [NotMapped] // no se encuentran en la tabla de BBDD
        public string? NombreUsuario {  get; set; }
        

    }
}
