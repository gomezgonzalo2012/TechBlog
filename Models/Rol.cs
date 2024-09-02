namespace TechBlog.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string? Nombre { get; set; }
        public IEnumerable<Usuario>? Usuarios { get; set; } // para mostrar lista de usuarios
    }
}
