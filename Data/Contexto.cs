namespace TechBlog.Data
{
    public class Contexto
    {
        public string Connection { get; }

        public Contexto(string con)
        {
            Connection = con;
        }
    }
}
