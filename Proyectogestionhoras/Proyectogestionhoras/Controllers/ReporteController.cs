using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyectogestionhoras.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ReporteService _reporteService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly UsuarioService usuarioService;
        public ReporteController(ReporteService reporteService,PROYECTO_CONTROL_HORASContext context, UsuarioService usuarioService)
        {
            _reporteService = reporteService;
            this.context = context;
            this.usuarioService = usuarioService;
        }

        public IActionResult TodosReportes()
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");
            
        }
        public async Task<IActionResult> ReportesFactura()
        {
            var facturas = await _reporteService.RecuperarFacturasProyectos();
            
            return View(facturas);
        }

        public async Task<IActionResult> pruebafactura()
        {
            var facturas = await _reporteService.RecuperarFacturasProyectos();
            ViewBag.Facturas = facturas;
            return View();
        }

        public async Task<IActionResult> ReporteHHSocios(string recurso)
        {
            var hhasignadas = await _reporteService.RecuperarHHProyectos(recurso);
            return View(hhasignadas);
        }
        public async Task<IActionResult> ReporteHHStaff(string recurso)
        {
            var hhasignadas = await _reporteService.RecuperarHHProyectos(recurso);
            return View(hhasignadas);
        }

        public async Task<IActionResult> ReporteHHConsultorA(string tipoconsultor)
        {
            var hhasignadas = await _reporteService.RecuperarHHConsultoresProyectos(tipoconsultor);
            return View(hhasignadas);
        }
        public async Task<IActionResult> ReporteHHConsultorB(string tipoconsultor)
        {
            var hhasignadas = await _reporteService.RecuperarHHConsultoresProyectos(tipoconsultor);
            return View(hhasignadas);
        }
        public async Task<IActionResult> ReporteHHConsultorC(string tipoconsultor)
        {
            var hhasignadas = await _reporteService.RecuperarHHConsultoresProyectos(tipoconsultor);
            return View(hhasignadas);
        }
        public async Task<IActionResult> ReporteCostosSocios(string recurso)
        {
            var costos = await _reporteService.RecuperarCostosProyectos(recurso);
            return View(costos);
        }
        public async Task<IActionResult> ReporteCostosStaff(string recurso)
        {
            var costos = await _reporteService.RecuperarCostosProyectos(recurso);
            return View(costos);
        }
        public async Task<IActionResult> ReporteCostosConsultorA(string tipoconsultor)
        {
            var costos = await _reporteService.RecuperarCostosConsultoresProyectos(tipoconsultor);
            return View(costos);
        }
        public async Task<IActionResult> ReporteCostosConsultorB(string tipoconsultor)
        {
            var costos = await _reporteService.RecuperarCostosConsultoresProyectos(tipoconsultor);
            return View(costos);
        }
        public async Task<IActionResult> ReporteCostosConsultorC(string tipoconsultor)
        {
            var costos = await _reporteService.RecuperarCostosConsultoresProyectos(tipoconsultor);
            return View(costos);
        }
        public async Task<IActionResult> ReporteMargenProyectos()
        {
            var margen = await _reporteService.RecuperarMargenProyectos();
            ViewBag.Margen = margen;
            return View();
        }
        public IActionResult SeleccionarProyecto()
        {
            var proyectos = context.Proyectos.Where(p=>p.StatusProyecto == 2);
            ViewBag.Proyectos = proyectos;
            return View();
        }
        [HttpPost]
        public IActionResult BuscarReporte(int idproyecto)
        {
            int proyecto = int.Parse(Request.Form["proyecto"].ToString());
            var numProyecto = context.Proyectos
            .Where(p => p.Id == proyecto) 
            .Select(p => p.NumProyecto) 
            .FirstOrDefault();
            var nombre = context.Proyectos
            .Where(p => p.Id == proyecto)
            .Select(p => p.Nombre)
            .FirstOrDefault();
            HttpContext.Session.SetString("numproyecto",numProyecto);
            HttpContext.Session.SetString("nombreproyecto", nombre);
            return RedirectToAction("ReporteControlAsignacionHH", new { idproyecto = proyecto });


        }
        public async Task<IActionResult> ReporteControlAsignacionHH(int idproyecto)
        {
            var controlhh = await _reporteService.ReporteControlHH(idproyecto);
            ViewBag.Controlhh = controlhh;
            return View();
        }

        public async Task<IActionResult> HHsociosActividad(int mes)
        {
            var horasmesactual = await _reporteService.HorasPorSociosMesActual(mes);
            var horasanioactual = await _reporteService.HorasPorSociosAnioActual(mes);
            var horasdocemeses = await _reporteService.HorasPorSociosUltimosDoceMeses(mes);
            ViewBag.Anioactual = horasanioactual;
            ViewBag.Docemeses = horasdocemeses;
            return View(horasmesactual);
        }
        public IActionResult ReporteProyectosNegociacion()
        {
            return View();
        }

        public async Task<IActionResult> ObtenerDatosNegociacion()
        {
            var proyectos = await _reporteService.ReporteProyectoNegociacion();
            return Json(proyectos);
        }
        public async Task<IActionResult> ReporteResumenQuarter(int anio)
        {
            
            var resumen = await _reporteService.RecuperarQuarter(anio);
            
            ViewBag.Resumen = resumen;
            return View();
        }
        [HttpPost]
        public IActionResult ConsultarResumenAnio()
        {
            int anios = int.Parse(Request.Form["anio"].ToString());
            HttpContext.Session.SetInt32("anio", anios);
            return RedirectToAction("ComparacionQuarter", new {anio=anios});
        }

        public async Task<IActionResult> ComparacionQuarter(int anio)
        {
            
            var resumen = await _reporteService.RecuperarQuarter(DateTime.Now.Year);
            ViewBag.Resumen = resumen;
            var resumenanio = await _reporteService.RecuperarQuarter(anio);
            ViewBag.ResumenA = resumenanio;
            return View();
        }

        public async Task<IActionResult> ReporteNeogiciacionKey()
        {
            var reporte = await _reporteService.ReporteNegociacionKeys();
            ViewBag.Negociacion = reporte;
            return View();
        }

        public async Task<IActionResult> recuperarcuotas(int mes,int anio)
        {
            var cuotas = await _reporteService.recuperarcuotasmensuales(mes, anio);
            var montofactura = await _reporteService.recuperarfacturamensual(mes, anio);
            ViewBag.Cuotas = cuotas;
            ViewBag.Montofactura = montofactura;
            return View();
        }
        public async Task<IActionResult> ReportehorasSociosProyectos(int mes,int anio)
        {
            var horasmaximas = await _reporteService.recuperarhhposibles(mes, anio);
            var horasproyectos = await _reporteService.recuperarhhproyectos(mes, anio);
            ViewBag.HorasProyectos = horasproyectos;
            ViewBag.HorasMaximas = horasmaximas;
            return View("recuperarhoras");
        }


        public async Task<IActionResult> PersonalChoice(int? idusuario,string?nombre,int? idrecurso)
        {
            var usuarios = await usuarioService.ObtenerUusario(idusuario,nombre,idrecurso);
            ViewBag.Usuarios = usuarios;
            return View();
        }


        /*DECARGAR BASE DE DATOS*/
        public async Task<ActionResult> DescargarBasedeDatos()
        {
            
            var proyectos = await _reporteService.ExportarBasedeDatos();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Proyectos");

                
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "unitt.png");
                var image = new FileInfo(logoPath);
                if (image.Exists)
                {

                    var picture = worksheet.Drawings.AddPicture("Logo", image);
                    picture.SetPosition(0, 0, 0, 0);

                }
                worksheet.Cells[3, 5].Value = "Base de Datos Proyectos Unit"; 
                worksheet.Cells[3, 5, 4, 13].Merge = true; 
                worksheet.Cells[3, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Centrar el texto
                worksheet.Cells[3, 5].Style.Font.Bold = true; 
                worksheet.Cells[3, 5].Style.Font.Size = 20; 

                worksheet.Cells[5, 1].Value = "Num.Proyecto";
                worksheet.Cells[5, 2].Value = "Proyecto";
                worksheet.Cells[5, 3].Value = "Cliente";
                worksheet.Cells[5, 4].Value = "U.Negocio";
                worksheet.Cells[5, 5].Value = "C.Costo";
                worksheet.Cells[5, 6].Value = "Código";
                worksheet.Cells[5, 7].Value = "IVA";
                worksheet.Cells[5, 8].Value = "Tipologia";
                worksheet.Cells[5, 9].Value = "Empresa";
                worksheet.Cells[5, 10].Value = "Monto";
                worksheet.Cells[5, 11].Value = "PLazo";
                worksheet.Cells[5, 12].Value = "Fecha Inicio";
                worksheet.Cells[5, 13].Value = "Fecha Término";
                worksheet.Cells[5, 14].Value = "HH_Socios";
                worksheet.Cells[5, 15].Value = "HH_Staff";
                worksheet.Cells[5, 16].Value = "HH_ConsultorA";
                worksheet.Cells[5, 17].Value = "HH_ConsultorB";
                worksheet.Cells[5, 18].Value = "HH_ConsultorC";
                worksheet.Cells[5, 19].Value = "CostoSocios";
                worksheet.Cells[5, 20].Value = "CostoStaff";
                worksheet.Cells[5, 21].Value = "CostoConsultorA";
                worksheet.Cells[5, 22].Value = "CostoConsultorB";
                worksheet.Cells[5, 23].Value = "CostoConsultorC";
                worksheet.Cells[5, 24].Value = "TotalServicios";
                worksheet.Cells[5, 25].Value = "TotalGastos";
                worksheet.Cells[5, 26].Value = "Status";

                using (var range = worksheet.Cells[5, 1, 5, 26])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    range.Style.Font.Bold = true;
                }


                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 15;
                worksheet.Column(10).Width = 15;
                worksheet.Column(11).Width = 15;
                worksheet.Column(12).Width = 15;
                worksheet.Column(13).Width = 15;
                worksheet.Column(14).Width = 15;
                worksheet.Column(15).Width = 15;
                worksheet.Column(16).Width = 15;
                worksheet.Column(17).Width = 15;
                worksheet.Column(18).Width = 15;
                worksheet.Column(19).Width = 15;
                worksheet.Column(20).Width = 15;
                worksheet.Column(21).Width = 15;
                worksheet.Column(22).Width = 15;
                worksheet.Column(23).Width = 15;
                worksheet.Column(24).Width = 15;
                worksheet.Column(25).Width = 15;
                worksheet.Column(26).Width = 15;
                

              
                int indice = 0;
                for (int i = 0; i < proyectos.Count(); i++)
                {
                    var planilla = proyectos[i];
                    indice = i + 6; 

                    worksheet.Cells[indice, 1].Value = planilla.NumProyecto;
                    worksheet.Cells[indice, 2].Value = planilla.NombreProyecto;
                    worksheet.Cells[indice, 3].Value = planilla.Cliente;
                    worksheet.Cells[indice, 4].Value = planilla.Unegocio;
                    worksheet.Cells[indice, 5].Value = planilla.CCosto;
                    worksheet.Cells[indice, 6].Value = planilla.Codigo;
                    worksheet.Cells[indice, 7].Value = planilla.AfectaIva;
                    worksheet.Cells[indice, 8].Value = planilla.Tipologia;
                    worksheet.Cells[indice, 9].Value = planilla.Empresa;
                    worksheet.Cells[indice, 10].Value = planilla.Monto;
                    worksheet.Cells[indice, 10].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 11].Value = planilla.Plazo;
                    worksheet.Cells[indice, 12].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[indice, 12].Value = planilla.Fechainicio?.ToString("dd/MM/yyyy");
                    worksheet.Cells[indice, 13].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[indice, 13].Value = planilla.Fechatermino?.ToString("dd/MM/yyyy");
                    worksheet.Cells[indice, 14].Value = planilla.HHsocios == 0 ? (object)null : planilla.HHsocios;
                    worksheet.Cells[indice, 15].Value = planilla.HHstaff == 0 ? (object)null : planilla.HHstaff;
                    worksheet.Cells[indice, 16].Value = planilla.HHConsultorA == 0 ? (object)null : planilla.HHConsultorA;
                    worksheet.Cells[indice, 17].Value = planilla.HHConsultorB == 0 ? (object)null : planilla.HHConsultorB;
                    worksheet.Cells[indice, 18].Value = planilla.HHConsultorC == 0 ? (object)null : planilla.HHConsultorC;
                    worksheet.Cells[indice, 19].Value = planilla.Costosocios == 0 ? (object)null : planilla.Costosocios;
                    worksheet.Cells[indice, 19].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 20].Value = planilla.Costostaff == 0 ? (object)null : planilla.Costostaff;
                    worksheet.Cells[indice, 20].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 21].Value = planilla.CostoconsultorA == 0 ? (object)null : planilla.CostoconsultorA;
                    worksheet.Cells[indice, 21].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 22].Value = planilla.CostoconsultorB == 0 ? (object)null : planilla.CostoconsultorB;
                    worksheet.Cells[indice, 22].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 23].Value = planilla.CostoconsultorC == 0 ? (object)null : planilla.CostoconsultorC;
                    worksheet.Cells[indice, 23].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 24].Value = planilla.TotalServicios == 0 ? (object)null : planilla.TotalServicios;
                    worksheet.Cells[indice, 24].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 25].Value = planilla.TotalGastos == 0 ? (object)null : planilla.TotalGastos;
                    worksheet.Cells[indice, 25].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 26].Value = planilla.Status;
                    
                }
                worksheet.View.FreezePanes(6, 3);
                string nombreArchivo = $"Base_Datos_Proyectos.xlsx";
                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
            }
        }
    }
}
