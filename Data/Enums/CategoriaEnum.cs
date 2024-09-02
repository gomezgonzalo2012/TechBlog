using System.ComponentModel;

namespace TechBlog.Data.Enums
{
    public enum CategoriaEnum
    {
        [Description("Notícias recientes")] // es lo que se va a poder mostrar en lugar del campo
        Noticias,
        [Description("Opiniones y comentarios")]
        Opinion,
        [Description("Novedades en tecnología")]
        Tecnologia,
        [Description("Guías y tutoriales")]
        Tutoriales,
        [Description("Recursos útiles")]
        Recursos
    }
}