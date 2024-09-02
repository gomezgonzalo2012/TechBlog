namespace TechBlog.Models.ViewModels
{
    public class PostDetallesViewModel
    {
        public Post? Post { get; set; }
        public List<Comentario>? ComentariosPrincipales { get; set; } // no tienen padre
        public List<Comentario>? ComentariosHijos { get; set; } // segundo nivel , con padre
        public List<Comentario>? ComentariosNietos { get; set; } // tercer nivel, con padre y abuelo
        public List<Post>? PostRecientes { get; set; } // para barra lateral con posts recientes
    }
}
