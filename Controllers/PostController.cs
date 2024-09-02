using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System;
using System.Data;
using TechBlog.Data;
using TechBlog.Models;
using TechBlog.Models.ViewModels;
using TechBlog.Servicios;

namespace TechBlog.Controllers
{
    public class PostController : Controller
    {
       private readonly Contexto _context;
       private readonly PostServicio _postServicio;

        // inyeccion por constructor
        public PostController(Contexto con)
        {
            _context = con;
            _postServicio = new PostServicio(con);
        }
        //---------------------------CREATE-----------------------------
        [Authorize(Roles="Administrador")]
        //GET
        public IActionResult Create()
        {
            return View(); // retorna vista create
            
        }
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        //POST
        public IActionResult Create(Post post)
        {
            using (var connection = new SqlConnection(_context.Connection))
            {
                connection.Open(); // abrir la coneccion
                using (var command = new SqlCommand("InsertarPost", connection)) // abrir comando hacia stored procedure
                {
                    command.CommandType = CommandType.StoredProcedure; // definimos el tipo del comando como stored procedure
                    // pasamos los parametros
                    command.Parameters.AddWithValue("@Titulo", post.Titulo); 
                    command.Parameters.AddWithValue("@Contenido", post.Contenido);
                    // De Enum lo pasamos a string como el dato en la db
                    command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                    // obtenemos la fecha actual
                    DateTime fechaActual = DateTime.UtcNow; 
                    command.Parameters.AddWithValue("@FechaCreacion", fechaActual);
                    command.ExecuteNonQuery(); // ejecutamos el procedimiento

                }// cierra el comando
            }// cierra la conexion
                return RedirectToAction("Index","Home"); // vista, controlador

        }

        // ------------------------------UPDATE--------------------------
        //GET
        [Authorize(Roles ="Administrador")]
        public IActionResult Update(int postId)
        {
            var postEdit= _postServicio.ObtenerPostPorId(postId);
            return View(postEdit); // envio a la vista Update con post a editar
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        //POST
        public IActionResult Post(Post post)
        {
            using (var connection = new SqlConnection(_context.Connection))
            {
                connection.Open();
                using (var command = new SqlCommand("ActualizarPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    command.Parameters.AddWithValue("@PostId",post.PostId);
                    command.Parameters.AddWithValue("@Titulo",post.Titulo);
                    command.Parameters.AddWithValue("@Contenido",post.Contenido);
                    command.Parameters.AddWithValue("@Categoria",post.Categoria.ToString());
                    // dejamos la fecha de creacion
                    command.ExecuteNonQuery();
                   
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // ----------------------------DELETE--------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Delete(int postId)
        {
            using( var connection = new SqlConnection( _context.Connection))
            {
                connection.Open();
                using (var command = new SqlCommand("EliminarPostPorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", postId);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // -------------------------------READ---------------------------
        // PROBAR HACERLO CON LISTAS DISTINTAS PARA COMENT PRINCIPAL, HIJOS Y NIETOS
        public IActionResult Details(int postId)
        {
            // en cada llamada se va agregando subcomentarios
            var postEdit = _postServicio.ObtenerPostPorId(postId);
            List<Comentario> comentarios = _postServicio.ObtenerComentariosPorPostId(postId);
            comentarios = _postServicio.ObtenerComentariosHijos(comentarios);
            comentarios = _postServicio.ObtenerComentariosNietos(comentarios);
            PostDetallesViewModel model = new PostDetallesViewModel()// contiene post y comentarios
            {
                Post = postEdit,
                // nos aseguramos que solo se muestren comentarios directos al post no respuestas a otros comentarios
                ComentariosPrincipales = comentarios.Where(c => c.ComentarioPadreId == null && c.ComentarioAbueloId == null).ToList(),
                // ahora verificamos que los comentarios hijos tengan padre y no tenga abuelo

                ComentariosHijos = comentarios.Where(c => c.ComentarioPadreId != null && c.ComentarioAbueloId == null).ToList(),
                // un comentario nieto tiene abuelo
                ComentariosNietos = comentarios.Where(c => c.ComentarioAbueloId != null).ToList(),

                // solo los 10 mas recientes posts
                PostRecientes = _postServicio.ObtenerPosts().Take(10).ToList()
            };
            
            return View(model);

        }

        // ---------------------------AGREAR COMENTARIO --------------------
        // NO NECESITA RESTRICCION 
        public IActionResult AgregarComentario (int postId, string comentario, int? comentariopadreid)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(comentario))// no aceptar comentarios nulos, vacios o solo con espacios
                {
                    ViewBag.Error = "El comentario no puede estar vacío.";
                    // recarga vista
                    return RedirectToAction("Details", "Post", new { id = postId });
                    // otra alternativa
                    // throw new Exception("El comentario no puede estar vacío");
                }

                // obtenemos el id del usuairo que inicio sesion
                int? userId = null;
                var userIdClaim = User.FindFirst("IdUsuario"); // IdUsuario se define al iniciar sesion
                if(userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                {
                    // si es distinto de nulo y si puede parsearse como entero
                    userId = parsedUserId;
                    DateTime FechaPublicacion = DateTime.UtcNow;

                    using (var connection = new SqlConnection(_context.Connection))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("AgregarComentario", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Contenido", comentario);
                            command.Parameters.AddWithValue("@FechaCreacion", FechaPublicacion);
                            command.Parameters.AddWithValue("@UsuarioId", userId);
                            command.Parameters.AddWithValue("@PostId", postId);
                            // comentarioPadreId puede ser nulo
                            command.Parameters.AddWithValue("@ComentarioPadreId", comentariopadreid ?? (object)DBNull.Value);
                            //connection.Open();
                            command.ExecuteNonQuery();
                            //connection.Close();
                        }
                    }
                }
                


                //se redirecciona a la accion details(necesita de postid) para que muestre la vista details
                return RedirectToAction("Details", "Post", new { id = postId });
            }
            catch (System.Exception ex)
            {// al suceder uan excepcion se recargará la pagina, mostrando el mendaje de error
                ViewBag.Error = ex.Message;
                // se redirecciona a la accion details (necesita de postid) para que muestre la vista details 
                return RedirectToAction("Details", "Post", new {id= postId}); // accion, controlador, objeto
            }

        }
    }
}
