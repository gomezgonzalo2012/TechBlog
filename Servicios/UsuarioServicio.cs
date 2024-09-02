using TechBlog.Data;

namespace TechBlog.Servicios
{
    public class UsuarioServicio
    {
        private readonly Contexto _contexto;

        public UsuarioServicio(Contexto contexto)
        {
            _contexto = contexto;
        }
    }
}
