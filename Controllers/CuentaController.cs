using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TechBlog.Data;
using TechBlog.Models;
using TechBlog.Servicios;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechBlog.Controllers
{
    // funciones solo de auth
    public class CuentaController : Controller
    {
        private readonly Contexto _context;
        private readonly UsuarioServicio _usuarioServicio;

        public CuentaController(Contexto context, UsuarioServicio usuarioServicio)
        {
            _context = context;
            _usuarioServicio = new UsuarioServicio(context);
        }

        // registro
        public IActionResult Registrar() // vista registro
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken] 
        // valida que las solicitudes provienen de la pagina web legitima y no de un sitio malicioso
        // impide ataques CSRF
        public IActionResult Registrar(Usuario model) // vista registro
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    using (SqlConnection conn= new SqlConnection(_context.Connection))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand("RegistrarUsuario", conn))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Nombre",model.Nombre);
                            command.Parameters.AddWithValue("@Apellido",model.Apellido);
                            command.Parameters.AddWithValue("@Correo", model.Correo);
                            // encriptamos la contraseña
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Contrasenia);
                            command.Parameters.AddWithValue("@Contrasenia",hashedPassword);
                            command.Parameters.AddWithValue("@NombreUsuario",model.NombreUsuario);
                            // generamos el token unico
                            var token = Guid.NewGuid();
                            command.Parameters.AddWithValue("@Token",token);
                            // expiracion del correo en 5 min
                            DateTime fechaExpiracion = DateTime.UtcNow.AddMinutes(5);
                            command.Parameters.AddWithValue("@FechaExpiracion",fechaExpiracion);
                            command.ExecuteNonQuery();

                            // PROGRAMAR ENVIO CORREO
                        }
                    }
                    return RedirectToAction("Token");  // acciion con vista para validar token


                }catch(SqlException ex) // es SQL por que se hace consulta a la BBDD para registrar
                {
                    // mensaje personalizado en base a codigo de error en SQL
                    if(ex.Number == 2627) // 2627 : ya existen dichos datos de usuario en la BBDD 
                    {
                        ViewBag.Error = "El correo electrónico y/o nombre de usuario ya se encuentra registrado";
                    }
                    else
                    {
                        ViewBag.Error = "Ocurrió un error al intentar registrar al usuario." + ex.Message;
                    }
                    throw;
                }
            }
            return View(model);
        }
    }
}
