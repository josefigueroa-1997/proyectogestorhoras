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
using Proyectogestionhoras.Services.Interface;
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
            var totalhhanuales = await context.TotalRecursos.Where(t => t.Anio == DateTime.Now.Year).ToListAsync();
            var costopromedio = await context.CostoPromedios.ToListAsync();
            await _usuarioService.GestorHHSocios();
            await _usuarioService.GestorHHStaff();
            ViewBag.TotalAnuales = totalhhanuales;
            ViewBag.Costos = costopromedio;
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
            if (usuario == null)
            {
                return NotFound(); 
            }
            ViewBag.Usuario = usuario;
            return View("EditarUsuario");
        }

        public async Task<IActionResult> EditarContrasena(int? idusuario, string? nombre, int? idrecurso)
        {
            var usuario = await _usuarioService.ObtenerUusario(idusuario, nombre, idrecurso);
            ViewBag.Usuario = usuario;
            return View();
        }

        public IActionResult ConfirmationSuccessfullPass()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarContrasena(Usuario usuario)
        {
            bool resultado = await _usuarioService.CambiarContrasena(usuario);
            if (resultado)
            {
                return RedirectToAction("ConfirmationSuccessfullPass");
            }
            else
            {
                return View();
            }
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

        [HttpPost]
        public async Task <IActionResult> ActualizarDatos(int idusuario, string nombre, string nombreusuario, string telefono, string email, int? hhsemanales, decimal costo, float? porcentaje, DateTime? fechainicio, DateTime? fechatermino, string nombrerecurso,string estado)
        {
            bool resultado = await _usuarioService.EditarUsuario(idusuario, nombre, nombreusuario, telefono, email, hhsemanales, costo, porcentaje, fechainicio, fechatermino,nombrerecurso,estado);
            if (resultado)
            {
                return RedirectToAction("Personal", "Usuario");
            }
            else
            {
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
                    int? idusuario = HttpContext.Session.GetInt32("id");
                    if (idusuario.HasValue)
                    {
                       string estado = await EstadoPersonal(idusuario.Value);
                        if (estado.Length > 0) {

                            if (estado == "Inactivo")
                            {
                                ViewBag.Warning = "Lo sentimos, pero ya no tiene acceso a esta plataforma. Si cree que esto es un error, por favor contacte con el soporte.";
                                return View();
                            }
                            
                        
                        }
                    }
                    
                    HttpContext.Session.SetString("nombre", login.Nombre);
                    HttpContext.Session.SetInt32("idrol", login.Rol);
                    HttpContext.Session.SetString("recurso", login.Recurso);
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

       public async Task<string> EstadoPersonal(int id)
        {
            try
            {
                var estado = await context.Usuarios.Where(u => u.Id == id).Join(context.Recursos,
                            u => u.IdRecurso,  
                            r => r.Id,         
                            (u, r) => r.Estado) 
                            .FirstOrDefaultAsync();
                return estado;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Hubo un error al recuperar el estado del usuario:{e.Message}");
                return null;
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
        public async Task<IActionResult> ObtenerdatosUsuarios()
        {
            var usuarios = await ( from u in context.Usuarios
                                   join r in context.Recursos on u.IdRecurso equals r.Id
                                   where (r.NombreRecurso == "Socio" || r.NombreRecurso == "Staff")
                                   select new PlanillaUsuarioDTO
                                   { 
                                    idusuario = u.Id,
                                    NombreUsuario = u.Nombre,
                                    recurso = r.NombreRecurso,
                                   }
                                   ).ToListAsync();
            
            return Json(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> RecuperarActividades(string recurso)
        {
            var actividades = await context.Actividades
                .Where(a => a.TipoAcatividad == recurso)
                .Select(a => new { a.Id, a.Nombre, a.Controlhh })
                .OrderBy(a => a.Id == 17 ? 0 : a.Id == 14 ? 1 : 2)
                .ThenBy(a => a.Nombre)
                .ToListAsync();
            return Json(actividades);
        }


        [HttpGet]
        public async Task<IActionResult> RecuperarActividadesEmpresa(string recurso)
        {
            var actividades = await context.Actividades
                           .Where(a => a.TipoAcatividad == recurso && a.Controlhh == "Empresa")
                           .Select(a => new { a.Id, a.Nombre })
                           .ToListAsync();
            return Json(actividades);
        }
        [HttpGet]
        public async Task<IActionResult> RecuperarSubActividades(int idactividad)
        {
            var subactividades = await context.Subactividads.Where(s => s.Idactividad == idactividad).
                Select(s => new { s.Id, s.Nombre }).ToListAsync();
            return Json(subactividades);
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerProyectoNegociacion()
        {
            var proyectos = await context.Proyectos.Where(p=>p.StatusProyecto==1).Select(p => new { p.Id, p.Nombre,p.NumProyecto }).ToListAsync();
            return Json(proyectos);
        }
    }

}



        






       
        