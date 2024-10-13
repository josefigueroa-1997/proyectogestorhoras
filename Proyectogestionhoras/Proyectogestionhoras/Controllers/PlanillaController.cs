using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using OfficeOpenXml;
using System.IO;

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
               
                var planilla = await planillaService.ObtenerPlanillaUsuario(id.Value,0);
                ViewBag.Planilla = planilla;
                
                return View("Planilla");

            }
            else
            {
                return View("Login","Usuario");
            }
            
        }
        
        public async Task<IActionResult> MisPlanillas(int? idusuario)
        {
            var planillas = await RecuperarMisPlanillas(idusuario);
            ViewBag.Planillas = planillas;
            return View("MisPlanillas");
        }

        public async Task<IActionResult> PlanillaMes(int idplanilla)
        {
            var planillames = await planillaService.ObtenerPlanillaExcel(idplanilla);
            ViewBag.Planilla = planillames;
            return View("PlanillaMes");
        }


        public async Task<ActionResult> ExportarExcel(int idplanilla)
        {
            var nombre = HttpContext.Session.GetString("nombre");
            var rol = HttpContext.Session.GetString("recurso");
            var planillas = await planillaService.ObtenerPlanillaExcel(idplanilla);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Planilla_Mes");

                worksheet.Cells[1, 1].Value = "Nombre:";
                worksheet.Cells[1, 2].Value = nombre;

             
                worksheet.Cells[2, 1].Value = "Rol:";
                worksheet.Cells[2, 2].Value = rol;

      
                string mes = "";
                int anio = 0;
                if (planillas.Count > 0)
                {
                    var primeraPlanilla = planillas[0];
                    mes = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(primeraPlanilla.FechaRegistro.Month);
                    anio = primeraPlanilla.FechaRegistro.Year;
                }

                worksheet.Cells[3, 1].Value = "Mes:";
                worksheet.Cells[3, 2].Value = mes;

              
                worksheet.Cells[4, 1].Value = "Año:";
                worksheet.Cells[4, 2].Value = anio;

             
                worksheet.Cells[5, 1].Value = "Fecha";
                worksheet.Cells[5, 2].Value = "Nombre Proyecto";
                worksheet.Cells[5, 3].Value = "Número Proyecto";
                worksheet.Cells[5, 4].Value = "Nombre de la Actividad";
                worksheet.Cells[5, 5].Value = "HH Registradas";
                worksheet.Cells[5, 6].Value = "Observaciones";

                decimal totalhoras = 0;
                int indice = 0;

                
                for (int i = 0; i < planillas.Count(); i++)
                {
                    var planilla = planillas[i];
                    indice = i + 6; 

                    worksheet.Cells[indice, 1].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[indice, 1].Value = planilla.FechaRegistro.Date.ToString("dd/MM/yyyy");
                    worksheet.Cells[indice, 2].Value = planilla.NombreProyecto;
                    worksheet.Cells[indice, 3].Value = planilla.NumProyecto;
                    worksheet.Cells[indice, 4].Value = planilla.NombreActividad;
                    worksheet.Cells[indice, 5].Value = planilla.HHregistradas;
                    worksheet.Cells[indice, 6].Value = planilla.Observaciones;

                    totalhoras += planilla.HHregistradas;
                }

       
                var style = worksheet.Cells[indice + 1, 4].Style;
                style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                worksheet.Cells[indice + 1, 4].Value = "Total";

                style = worksheet.Cells[indice + 1, 5].Style;
                style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);

                worksheet.Cells[indice + 1, 5].Value = totalhoras;
                string nombreArchivo = $"planilla_{nombre}_{mes}_{anio}.xlsx";

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
            }
        }





        public async Task<List<Planilla>> RecuperarMisPlanillas(int? idusuario)
        {
            var planillas = await context.Planillas
            .Where(p => p.IdUsuario == idusuario)
             .ToListAsync();
            return planillas;
        }



        [HttpPost]
        public async Task<IActionResult> RegistrarHoras(int idusuario, int idusuproy, int horasasignadas, DateTime Fecharegistro, string? observaciones, int idactividad)
        {
            bool registroExitoso = false;
            bool yaSeRegistraronHoras = false;
            bool horasExcedidas = false;

            try
            {
                // Llamar al servicio que registra las horas
                int resultado = await planillaService.RegistrarHoras(idusuario, idusuproy, horasasignadas, Fecharegistro, observaciones, idactividad);

                if (resultado == 1)
                {
                    registroExitoso = true;
                }
                else if (resultado == 2)
                {
                    yaSeRegistraronHoras = true;
                }
                else if (resultado == 3)
                {
                    horasExcedidas = true;
                }
            }
            catch (Exception ex)
            {
                // Manejar errores inesperados
                return Json(new { success = false, message = "Ocurrió un error inesperado: " + ex.Message });
            }

            // Devolver la respuesta basada en el resultado
            if (registroExitoso)
            {
                return Json(new { success = true, message = "Horas registradas exitosamente." });
            }
            else if (yaSeRegistraronHoras)
            {
                return Json(new { success = false, message = "Ya se han registrado horas para este proyecto en esta fecha." });
            }
            else if (horasExcedidas)
            {
                return Json(new { success = false, message = "No se pueden registrar más horas en esta semana, se ha excedido el límite permitido." });
            }
            else
            {
                return Json(new { success = false, message = "Ocurrió un error inesperado." });
            }
        }


    }
}
