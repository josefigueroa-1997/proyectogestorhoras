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
using Microsoft.EntityFrameworkCore;
namespace Proyectogestionhoras.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public UsuarioController(UsuarioService usuarioService,PROYECTO_CONTROL_HORASContext context)
        {
            _usuarioService = usuarioService;
            this.context = context;
            
        }

        public async Task<IActionResult> Personal(int? id,string? nombre,int? id_recurso)
        {
            var usuarios = await _usuarioService.ObtenerUusario(id,nombre, id_recurso);
            ViewBag.Usuarios = usuarios;
            return View();
        }

        public async Task< IActionResult> AddPersonal() {
            var roles = await _usuarioService.ObtenerRoles();
         
            ViewBag.Roles = roles;
 
            return View();
        }

        public async Task<IActionResult> EditarDatosUsuario(int? idusuario,string? nombre,int? idrecurso)
        {
            var usuario = await _usuarioService.ObtenerUusario(idusuario,nombre,idrecurso);
            ViewBag.Usuario = usuario;
            return View("EditarUsuario");
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario(string nombre, string rut, string telefono, string email, int idrol, string nombrerecurso, int? numhoras, decimal costounitario, float? porcentajehoras, DateTime? fechainicio, DateTime? fechafin)
        {
            try
            {
                bool resultado = await _usuarioService.RegistrarUsuario(nombre,rut,telefono,email,idrol,nombrerecurso,numhoras,costounitario,porcentajehoras,fechainicio,fechafin);
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

        [HttpGet]
        public async Task<IActionResult> VerificarCorreo(string email)
        {
            int resultado = await _usuarioService.VerificarCorreo(email);
            return Ok(resultado);
        }
        [HttpGet]
        public async Task<IActionResult> VerificarRut(string rut)
        {
            int resusltado = await _usuarioService.Verificarrut(rut);
            return Ok(resusltado);
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
                var login = await _usuarioService.ObtenerCredenciales(rut);
                if (login is null)
                {
                    ViewBag.Error = "Rut y/o Contraseña Incorrectas";
                    return View();
                }
                bool resultado = await _usuarioService.Login(rut, contrasena);
               
                if (resultado)
                {
                    HttpContext.Session.SetInt32("id", login.Id);
                    HttpContext.Session.SetString("nombre", login.Nombre);
                    HttpContext.Session.SetInt32("idrol", login.Rol);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Rut y/o Contraseña Incorrectas";
                    return View();
                }
               
            
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

        
        
        [HttpGet]      
        public async Task<IActionResult> ObtenerMisproyectos(int idusuario)
        {
            var proyectos = await _usuarioService.ObtenerUsuariosProyecto(idusuario);
            return Json(proyectos);
        }

        [HttpGet]
        public async Task<IActionResult> Recuperarhhusuarios(int idusuario,int idproyecto)
        {
            var informacionusuario = await _usuarioService.RecuperarHHUsuarios(idusuario, idproyecto);
            return Json(informacionusuario);
        }

        [HttpGet]
        public async Task<IActionResult> RecuperarActividades(string recurso)
        {
            var actividades = await context.Actividades
                           .Where(a => a.TipoAcatividad == recurso)
                           .Select(a => new { a.Id, a.Nombre })
                           .ToListAsync();
            return Json(actividades);
        }


    }

}



        






       
        