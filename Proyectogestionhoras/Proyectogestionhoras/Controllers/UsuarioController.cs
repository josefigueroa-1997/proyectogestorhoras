using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace Proyectogestionhoras.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly Conexion conexion;
        public UsuarioController(UsuarioService usuarioService,Conexion conexion)
        {
            _usuarioService = usuarioService;
            this.conexion = conexion;
        }

        public async Task<IActionResult> Personal(int? id,string? nombre,int? id_recurso)
        {
            var usuarios = await _usuarioService.ObtenerUusario(id,nombre, id_recurso);
            ViewBag.Usuarios = usuarios;
            return View();
        }

        public async Task< IActionResult> AddPersonal() {
            var roles = await _usuarioService.ObtenerRoles();
            var segmentos = await _usuarioService.ObtenerSegmentoCosto();
            ViewBag.Roles = roles;
            ViewBag.Segmentos = segmentos;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario(string nombre, string rut, string telefono, string email, int idrol, string nombrerecurso, int numhoras, decimal costounitario, int idsegmento)
        {
            try
            {
                bool resultado = await _usuarioService.RegistrarUsuario(nombre,rut,telefono,email,idrol,nombrerecurso,numhoras,costounitario,idsegmento);
                if (resultado)
                {
                    TempData["SuccessMessage"] = "¡Se Agregó con éxito el nuevo Personal!";
                    return RedirectToAction("Personal","Usuario");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return View();


            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string rut, string contrasena)
        {
            try
            {
                if (!string.IsNullOrEmpty(rut) && !string.IsNullOrEmpty(contrasena))
                {

                    Login credenciales = new Login();
                    credenciales = await ObtenerCredenciales(rut);
                    if (credenciales != null)
                    {
                        bool validar = VerificarContrasena(contrasena, credenciales.Contraseña);
                        if (validar)
                        {
                            HttpContext.Session.SetInt32("id",credenciales.Id);
                            HttpContext.Session.SetString("nombre", credenciales.Nombre);
                            HttpContext.Session.SetInt32("idrol", credenciales.Rol);
                            return RedirectToAction("Index", "Home", new {id=credenciales.Id});
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Contraseña Incorrecta";
                        return View();
                    }

                }
                else
                {
                    ViewBag.Error = "Debe ingresar su rut y/o contraseña";
                    return View();
                }
                return View();
            
            }
            catch (Exception e)
            {
                Debug.WriteLine($"hubo un error al iniciar sesión" + e.Message);
                return View();
            }
        }
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();


            return RedirectToAction("Index", "Home");
        }

        private async Task<Login> ObtenerCredenciales(string correo)
        {
            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
            using (DbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INICIOSESION";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CORREO", correo));
                Login usuario = new Login();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        usuario = new Login()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID")),
                            Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                            Rol = reader.GetInt32(reader.GetOrdinal("ROL")),
                            Contraseña = reader.GetString(reader.GetOrdinal("CONTRASEÑA")),
                        };
                    }
                }
                
                return usuario;
            }
        }
        private Boolean VerificarContrasena(string passuser, string passbd)
        {
            try
            {
                bool verificar = BCrypt.Net.BCrypt.Verify(passuser, passbd);
                return verificar;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al verificar las contraseñas:{e.Message}");
                return false;
            }
        }



    }

}



        






       
        