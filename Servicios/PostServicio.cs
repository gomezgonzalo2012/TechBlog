using Microsoft.Data.SqlClient;
using TechBlog.Data;
using TechBlog.Models;
using System.Data;
using TechBlog.Data.Enums;
using Microsoft.Extensions.Hosting;

namespace TechBlog.Servicios
{
    public class PostServicio
    {
        private readonly Contexto _contexto;
        public PostServicio(Contexto con)
        {
            _contexto = con;
        }

        public Post ObtenerPostPorId (int postId)
        {
            Post post = new Post();

            using (var connection = new SqlConnection(_contexto.Connection))
            {
                connection.Open();
                using (var command = new SqlCommand("ObtenerPostPorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", postId);
                    using (SqlDataReader reader = command.ExecuteReader()) // obtiene datos enviados desde la consulta
                    {
                        if(reader.Read()) // validar que el reder lea datos
                        {
                            post.PostId = (int)reader["PostId"];
                            post.Titulo = (string)reader["Titulo"];
                            post.Contenido = (string)reader["Contenido"];
                            //post.Categoria = (CategoriaEnum)reader["Categoria"];
                            post.Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]);
                            post.FechaCreacion = (DateTime)reader["FechaCreacion"];
                        }
                    }
                }
            }
            return post; // retorna vacío si no lo encuentra en BBDD
        }

        public List<Post> ObtenerPosts()
        {
            List<Post> posts = new List<Post>();
            using (var connection = new SqlConnection(_contexto.Connection))
            {
                connection.Open();
                using(var command = new SqlCommand("ObtenerTodosLosPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using(SqlDataReader reader = command.ExecuteReader()) { 

                        while(reader.Read()) // mientras exista algo que leer
                        {
                            posts.Add(new Post // rellenamos la lista
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                            //post.Categoria = (CategoriaEnum)reader["Categoria"];
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"]
                            }
                            );
                        }
                    }
                }
            }
            return posts;
        }

        public List<Comentario> ObtenerComentariosPorPostId( int postId)
        {
            //c.ComentarioId, c.Contenido, c.FechaCreacion, c.UsuarioId, c.PostId, u.NombreUsuario
            List<Comentario> comentarios = new List<Comentario> ();
            using ( SqlConnection connection= new SqlConnection(_contexto.Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerComentariosPorPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", postId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            comentarios.Add(new Comentario
                            {
                                ComentarioId = (int)reader["ComentarioId"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                UsuarioId = (int)reader["UsuarioId"],
                                PostId = (int)reader["PostId"],
                                NombreUsuario = (string)reader["NombreUsuario"]
                            });
                        }
                    }
                }
            }
            return comentarios;
        }

        // por cada comentario principal se consultará por sus comentarios hijos, por eso el parametro es List
        public List<Comentario> ObtenerComentariosHijos(List<Comentario> comentarios) 
        {
          
            foreach(var commentPrincipal in comentarios) // itera por cada comentario principañ
            {
                using (SqlConnection connection = new SqlConnection(_contexto.Connection))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("ObtenerComentariosHijosPorComentarioId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ComentarioId", commentPrincipal.ComentarioId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Comentario> listaComentariosHijos = new List<Comentario>();
                            while (reader.Read())
                            {
                               var comentarioHijo =  new Comentario
                                {
                                    ComentarioId = (int)reader["ComentarioId"],
                                    Contenido = (string)reader["Contenido"],
                                    FechaCreacion = (DateTime)reader["FechaCreacion"],
                                    UsuarioId = (int)reader["UsuarioId"],
                                    PostId = (int)reader["PostId"],
                                    NombreUsuario = (string)reader["NombreUsuario"],
                                    ComentarioPadreId = commentPrincipal.ComentarioId
                                };
                                listaComentariosHijos.Add(comentarioHijo);
                            }
                            commentPrincipal.ComentariosHijos = listaComentariosHijos; //
                        }
                    }
                }
            }
            
            return comentarios;
        }

        public List<Comentario> ObtenerComentariosNietos(List<Comentario> comentarios)
        {

            foreach (var comentarioPrincipal in comentarios) // itera por cada comentario principal
            {
                if (comentarioPrincipal.ComentariosHijos != null)
                {
                    foreach (var comentarioHijo in comentarioPrincipal.ComentariosHijos) // itera por cada comentario hijo
                    {
                        using (SqlConnection connection = new SqlConnection(_contexto.Connection))
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("ObtenerComentariosHijosPorComentarioId", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@ComentarioId", comentarioHijo.ComentarioId);
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    List<Comentario> listaComentariosNietos = new List<Comentario>();
                                    while (reader.Read())
                                    {
                                        var comentarioNieto = new Comentario
                                        {
                                            ComentarioId = (int)reader["ComentarioId"],
                                            Contenido = (string)reader["Contenido"],
                                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                                            UsuarioId = (int)reader["UsuarioId"],
                                            PostId = (int)reader["PostId"],
                                            NombreUsuario = (string)reader["NombreUsuario"],
                                            ComentarioPadreId = comentarioHijo.ComentarioId,
                                            ComentarioAbueloId = comentarioPrincipal.ComentarioId
                                        };
                                        listaComentariosNietos.Add(comentarioNieto);
                                    }
                                    comentarioHijo.ComentariosHijos = listaComentariosNietos;
                                }
                            }
                        }

                    }

                }
                
                
            }

            return comentarios;
        }

        public List<Post> ObtenerPostPorCategoria(CategoriaEnum categoria)
        {
            var listaPostCategoria = new List<Post>();
            using (SqlConnection connection = new SqlConnection(_contexto.Connection))
            {
                connection.Open();
                using(  SqlCommand command = new SqlCommand("ObtenerPostPorCategoria", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Categoria", categoria);
                    using( SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Contenido = (string)reader["Contenido"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                Titulo = (string)reader["Titulo"]
                            };
                            listaPostCategoria.Add(post);   
                        }
                        
                    }
                }
            }
            return listaPostCategoria;
        }
        public List<Post> ObtenerPostPorTitulo(string titulo)
        {
            var listaPostTitulo = new List<Post>();
            using (SqlConnection connection = new SqlConnection(_contexto.Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("ObtenerPostPorTitulo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Titulo", titulo);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaPostTitulo.Add(new Post
                            {
                                PostId = (int)reader["PostId"],
                                Contenido = (string)reader["Contenido"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                Titulo = (string)reader["Titulo"]
                            });
                            
                        }

                    }
                }
            }
            return listaPostTitulo;
        }
    }
}
