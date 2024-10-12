using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;

namespace Proyectogestionhoras.Controllers
{
    public class PlanillaController : Controller
    {
        private readonly PlanillaService planillaService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public PlanillaController(PlanillaService planillaService,PROYECTO_CONTROL_HORASContext context)
        {
            this.planillaService = planillaService;
            this.context = context;
        }
        public async Task<IActionResult> PlanillaRegistro()
        {
            var id = HttpContext.Session.GetInt32("id");
            if(id.HasValue)
            {
               
                var planilla = await planillaService.ObtenerPlanillaUsuario(id.Value);
                ViewBag.Planilla = planilla;
                
                return View("Planilla");

            }
            else
            {
                return View("Login","Usuario");
            }
            
        }
        public async Task<List<Actividade>> RecuperarActividades()
        {
            var actividades = await context.Actividades.ToListAsync();
            return actividades;
        }
        public async Task<List<Proyecto>> RecuperarProyectos()
        {
            var proyectos = await context.Proyectos.ToListAsync(); return proyectos;
        }

        [HttpPost]
        public async Task <IActionResult> RegistrarHoras(int idusuario, int idusuproy, int horasasignadas, DateTime Fecharegistro, string? observaciones, int idactividad)
        {
        
            bool registroExitoso = false;
            bool yaSeRegistraronHoras = false;


            try
            {
            
                int resultado = await planillaService.RegistrarHoras(idusuario, idusuproy, horasasignadas, Fecharegistro, observaciones, idactividad);

    
                if (resultado == 1)
                {
                    registroExitoso = true;
                }
                else if (resultado == 2)
                {
                    yaSeRegistraronHoras = true;
                }
      
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = "Ocurrió un error inesperado: " + ex.Message });
            }

    
            if (registroExitoso)
            {
                return Json(new { success = true, message = "Horas registradas exitosamente." });
            }
            else if (yaSeRegistraronHoras)
            {
                return Json(new { success = false, message = "Ya se han registrado horas para este proyecto en esta fecha." });
            }
            else
            {
                return Json(new { success = false, message = "Ocurrió un error inesperado." });
            }
        }


    }
}
