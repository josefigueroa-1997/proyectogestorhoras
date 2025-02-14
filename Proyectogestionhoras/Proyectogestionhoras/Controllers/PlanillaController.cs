using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using OfficeOpenXml;
using System.IO;
using System.Diagnostics;
using Proyectogestionhoras.Models.DTO;
using iText.Html2pdf;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using iText.Layout;
using System.Globalization;

namespace Proyectogestionhoras.Controllers
{
    public class PlanillaController : Controller
    {
        private readonly PlanillaService planillaService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly UsuarioService usuarioService;
        private readonly ICompositeViewEngine _viewEngine;
        public PlanillaController(PlanillaService planillaService,PROYECTO_CONTROL_HORASContext context,UsuarioService usuarioService, ICompositeViewEngine viewEngine)
        {
            this.planillaService = planillaService;
            this.context = context;
            this.usuarioService = usuarioService;
            this._viewEngine = viewEngine;
        }
        public async Task<IActionResult> PlanillaRegistro()
        {
            var id = HttpContext.Session.GetInt32("id");
            if(id.HasValue)
            {
              
                var planilla = await planillaService.ObtenerPlanillaUsuario(id.Value,0);
                int mesActual = DateTime.Now.Month;
                int anioActual = DateTime.Now.Year;

                var planillaFiltrada = planilla
                    .Where(p => p.Mes == mesActual && p.Anio == anioActual)
                    .ToList();

                ViewBag.Planilla = planillaFiltrada;

                return View("Planilla");

            }
            else
            {
                return View("Login","Usuario");
            }
            
        }

        public  IActionResult SeleccionarPlanillaHoras()
        {
            var id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {
                
                return View();

            }
            else
            {
                return View("Login", "Usuario");
            }

        }

        public async Task<IActionResult> PlanillaEmpresa()
        {
            var id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {
                var planilla = await planillaService.ObtenerPlanillaEmpresaUsuario(id.Value, 0);
                ViewBag.PlanillaEmpresa = planilla;

                return View();

            }
            else
            {
                return View("Login", "Usuario");
            }

        }

        [HttpPost]
        public async Task<IActionResult> RegistrarHoras(int idusuario, int? idusuproy, string horasasignadas, DateTime Fecharegistro, string? observaciones, int Idactividad,int? idsubactividad)
        {
            bool registroExitoso = false;
            bool yaSeRegistraronHoras = false;
            bool horasExcedidas = false;
            bool fechafuerarango = false;
            bool error = false;

            try
            {

                int resultado = await planillaService.RegistrarHorasPlanills(idusuario, idusuproy, horasasignadas,  Fecharegistro,  observaciones,  Idactividad,  idsubactividad);

                if (resultado == 1)
                {
                    registroExitoso = true;
                }
                else if (resultado == 2)
                {
                    yaSeRegistraronHoras = true;
                }
                else if(resultado == 0)
                {
                    error = true;
                }
                /* else if (resultado == 3)
                 {
                     horasExcedidas = true;
                 }
                 else if(resultado == 4)
                 {
                     fechafuerarango = true;
                 }*/
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
                return Json(new { success = false, message = "Ya se han registrado horas para esta actividad en este proyecto durante esta fecha." });
            }
            else if (error)
            {
                return Json(new { success = false, message = "Ocurrió un error al registrar las horas." });
            }
            /*else if (horasExcedidas)
              {
                  return Json(new { success = false, message = "No se pueden registrar más horas en esta semana, se ha excedido el límite permitido." });
              }
              else if (fechafuerarango)
              {
                  return Json(new { success = false, message = "Error en el registro.La fecha de registro tiene que estar en el rango de fecha de la ejecución del proyecto." });
              }*/
            else
            {
                return Json(new { success = false, message = "Ocurrió un error inesperado." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarHorasEmpresa(int idusuario, string horasasignadas, DateTime Fecharegistro, string? observaciones, int idsubactividad)
        {
            bool registroExitoso = false;
            bool yaSeRegistraronHoras = false;
            

            try
            {

                int resultado = await planillaService.RegistrarHorasEmpresa(idusuario, horasasignadas, Fecharegistro, observaciones, idsubactividad);

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
                return Json(new { success = false, message = "Ya se han registrado horas para esta actividad en este proyecto durante esta fecha." });
            }
            
            else
            {
                return Json(new { success = false, message = "Ocurrió un error inesperado." });
            }
        }



        public async Task<IActionResult> MisPlanillas(int? idusuario)
        {
            var planillas = await context.Planillas
                .Where(p => p.IdUsuario == idusuario)
                .OrderBy(p => p.Anio)
                .ThenBy(p => p.Mes)
                .ToListAsync();

                 var planillasAgrupadas = planillas
                .GroupBy(p => p.Anio)
                .Select(g => new
                {
                    Anio = g.Key,
                    Meses = g.Select(p => new { p.Id, p.Mes }).ToList()
                })
                .ToList();

            ViewBag.PlanillasPorAnio = planillasAgrupadas;

            return View("MisPlanillas");
        }

        public async Task<IActionResult> PlanillaMes(int idplanilla)
        {
            var planillames = await planillaService.ObtenerPlanillaExcel(idplanilla);
            ViewBag.Planilla = planillames;
            return View("PlanillaMes");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarRegistroPlanilla()
        {
            int idregistro = int.Parse(Request.Form["idregistro"].ToString());
            int planilla = await context.PlanillaRegistroEmpresas.Where(p => p.Id == idregistro).Select(p => p.IdPlanilla).FirstOrDefaultAsync();
            int planillaproyecto = await context.PlanillaUsusarioProyectos.Where(p => p.Id == idregistro).Select(p => p.IdPlanilla).FirstOrDefaultAsync();
            int planillaoficial = 0;

            if (planilla > 0)
            {
                planillaoficial = planilla;
            }
            else if (planillaproyecto > 0)
            {
                planillaoficial = planillaproyecto;
            }
            try
            {
                bool resultado = await planillaService.EliminarRegistro(idregistro);
                if (resultado)
                {

                    TempData["SuccessMessageEliminacion"] = "El registro se ha eliminado correctamente.";
                    return RedirectToAction("PlanillaMes", "Planilla", new { idplanilla = planillaoficial });


                }
                else
                {
                    Debug.WriteLine($"error al eliminar");
                }
            }
            catch(Exception e)
            {
                TempData["ErrorMessageEliminacion"] = "El registro no se ha podido eliminar.";
                return RedirectToAction("PlanillaMes", "Planilla", new { idplanilla = planillaoficial });
            }


            return RedirectToAction("PlanillaMes", "Planilla", new { idplanilla = planillaoficial });
        }

        public async Task<ActionResult> ExportarExcel(int idplanilla)
        {
           // var nombre = HttpContext.Session.GetString("nombre");
            //var rol = HttpContext.Session.GetString("recurso");
            var planillas = await planillaService.ObtenerPlanillaExcel(idplanilla);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Planilla_Mes");

                worksheet.View.ShowGridLines = false;
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "unitt.png");
                var image = new FileInfo(logoPath);
                if (image.Exists)
                {
                 
                    var picture = worksheet.Drawings.AddPicture("Logo", image);
                    picture.SetPosition(0, 0, 0, 0); 
                                                     
                }

              
                worksheet.Cells["A1:B1"].Merge = true; 
                worksheet.Cells["A1"].Value = "";

                
                

                

                string mes = "";
                string anio = "";
                string nombre = "";
                string rol = "";
                if (planillas.Count > 0)
                {
                    var primeraPlanilla = planillas[0];
                    mes = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(primeraPlanilla.FechaRegistro.Month);
                    anio ="'" + primeraPlanilla.FechaRegistro.Year.ToString();
                    nombre = primeraPlanilla.NombreUsuario;
                    rol = primeraPlanilla.Rol;
                }
                worksheet.Cells[5, 1].Value = "Nombre:";
                worksheet.Cells[5, 2].Value = nombre;
                
                worksheet.Cells[6, 1].Value = "Rol:";
                worksheet.Cells[6, 2].Value = rol;

                worksheet.Cells[7, 1].Value = "Mes:"; 
                worksheet.Cells[7, 2].Value = mes;

                worksheet.Cells[8, 1].Value = "Año:"; 
                worksheet.Cells[8, 2].Value = anio;

                worksheet.Cells[9, 1].Value = ""; 

              
                worksheet.Cells[10, 1].Value = "Fecha"; 
                
                worksheet.Cells[10, 2].Value = "Nombre de la Actividad";
                worksheet.Cells[10, 3].Value = "Proyecto";
               
                worksheet.Cells[10, 4].Value = "HH Registradas";
                worksheet.Cells[10, 5].Value = "Costo Unitario";
                worksheet.Cells[10, 6].Value = "Costo Total";
                worksheet.Cells[10, 7].Value = "Observaciones";
               
                
                /*worksheet.Cells[10, 9].Value = "Segmento";*/

                using (var rango = worksheet.Cells[10, 1, 10, 7])
                {
                    rango.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rango.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    rango.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    rango.Style.Font.Bold = true;
                }

         
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 40;
                
                /*worksheet.Column(9).Width = 40;*/

                decimal totalhoras = 0;
                decimal totalcosto = 0;
                int indice = 0;
                for (int i = 0; i < planillas.Count(); i++)
                {
                    var planilla = planillas[i];
                    indice = i + 11; 

                    worksheet.Cells[indice, 1].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[indice, 1].Value = planilla.FechaRegistro.Date.ToString("dd/MM/yyyy");
                    worksheet.Cells[indice, 2].Value = planilla.NombreActividad;
                    worksheet.Cells[indice, 3].Value = planilla.Nombre;
                    worksheet.Cells[indice, 4].Value = planilla.HHregistradas;
                    worksheet.Cells[indice, 5].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 5].Value = planilla.CostoUnitario;
                    worksheet.Cells[indice, 6].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 6].Value = planilla.CostoTotal;
                    worksheet.Cells[indice, 7].Value = planilla.Observaciones;
                    /*worksheet.Cells[indice, 9].Value = planilla.NombreSegmento;*/
                    totalhoras += planilla.HHregistradas;
                    totalcosto += planilla.CostoTotal;
                }

                var range = worksheet.Cells[indice + 1, 1, indice + 1, 8];
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                range.Style.Font.Bold = true;

                worksheet.Cells[indice + 1, 3].Value = "Totales";
                worksheet.Cells[indice + 1, 4].Value = totalhoras;
                worksheet.Cells[indice + 1,6].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[indice + 1, 6].Value = totalcosto;

                string nombreArchivo = $"planilla_{nombre}_{mes}_{anio}.xlsx";
                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
            }
        }

        public async Task<IActionResult> GanttUsuario()
        {
            int? idusuario = HttpContext.Session.GetInt32("id");
            var proyectoshh = await planillaService.RecuperarHHUsuarios(idusuario.Value);
            ViewBag.Proyectos = proyectoshh;
            return View("GanttUsuario");
        }
        private async Task<string> RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, true);

                // Comprobar si la vista fue encontrada
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException("viewResult.View", "No se encontró la vista.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.ToString();
            }
        }

        public async Task<IActionResult> GenerarPDF()
        {

            var html = await RenderViewToString("GanttUsuario", null);

            using (var memoryStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(html, memoryStream);
                return File(memoryStream.ToArray(), "application/pdf", "mi_gantt.pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerarGant(int idusuario)
        {
            try
            {
           
                var resultado = await planillaService.ObtenerDatosGantt(idusuario);


             
                var gantData = resultado.Select(p => new
                {
                    NombreProyecto = p.NombreProyecto,
          
                    FechaInicio = p.FechaInicio,
                    FechaTermino = p.FechaTermino,
                    HorasPorMes = p.HorasPorMes.Select(h => new
                    {
                        Año = h.Año,
                        Mes = h.Mes.ToString("D2"), 
                        Horas = h.Horas
                    }).ToList()
                }).ToList();

                return Json(gantData);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Debug.WriteLine($"Hubo un error al generar el Gantt: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al generar el gráfico de Gantt.");
            }
        }

    }
}
