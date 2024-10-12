using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Services;

namespace Proyectogestionhoras.Controllers
{
    public class PlanillaController : Controller
    {
        private readonly PlanillaService planillaService;

        public PlanillaController(PlanillaService planillaService)
        {
            this.planillaService = planillaService;
        }
        public IActionResult PlanillaRegistro()
        {
            var id = HttpContext.Session.GetInt32("id");
            if(id.HasValue)
            {
                return View("Planilla");

            }
            else
            {
                return View("Login","Usuario");
            }
            
        }

        [HttpPost]
        public async Task <IActionResult> RegistrarHoras(int idusuario, int idusuproy, int horasasignadas, DateTime Fecharegistro, string? observaciones, int idactividad)
        {
            // Lógica para registrar horas
            bool registroExitoso = false;
            bool yaSeRegistraronHoras = false;

            // Aquí deberías implementar la lógica real para registrar horas
            try
            {
                // Llama al servicio que se encarga de registrar las horas
                int resultado = await planillaService.RegistrarHoras(idusuario, idusuproy, horasasignadas, Fecharegistro, observaciones, idactividad);

                // Manejo de resultados del servicio
                if (resultado == 1)
                {
                    registroExitoso = true;
                }
                else if (resultado == 2)
                {
                    yaSeRegistraronHoras = true;
                }
                // Aquí podrías manejar otros resultados si los hubiera.
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                return Json(new { success = false, message = "Ocurrió un error inesperado: " + ex.Message });
            }

            // Respuesta al cliente
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
