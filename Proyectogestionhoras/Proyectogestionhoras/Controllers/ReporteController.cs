using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using System.Diagnostics;
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
            var idproyectos = context.Proyectos
            .Where(p => p.Id == proyecto)
            .Select(p => p.Id)
            .FirstOrDefault();
            HttpContext.Session.SetString("numproyecto",numProyecto);
            HttpContext.Session.SetString("nombreproyecto", nombre);
            HttpContext.Session.SetInt32("IDproyecto", idproyectos);
            return RedirectToAction("ReporteControlAsignacionHH", new { idproyecto = proyecto });


        }
        public IActionResult ReporteControlAsignacionHH()
        {
            
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerControlHH(int idproyecto)
        {
            var resultado = await _reporteService.ReporteControlHH(idproyecto);
            return Json(resultado);
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


        public IActionResult ReporteBalanceHoras()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RecuperarBalanceHoras()
        {
            var balance = await _reporteService.RecuperarBalanceHoras();
            return Json(balance);
        }


        public IActionResult ReporteForecastCostos()
        {
            return View();
        }

        public IActionResult ReporteForecastIngresos()
        {
            return View();
        }

        public async Task <IActionResult> ReporteForecastFlujoCaja(int? idproyecto)
        {
            var datos = await _reporteService.ProcesarFlujoCajaPorMesAsync(idproyecto);
            return View(datos);
        }

        public IActionResult ReporteResumenCostosProyecto()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerCostosProyecto()
        {
            var resumen = await _reporteService.ObtenerResumenCostosProyecto();
            return Json(resumen);
        }


        /*EXPORTAR FLUJO DE CAJA*/
        public async Task<IActionResult> ExportarFlujoCaja(int? idproyecto)
        {
            try
            {
                var model = await _reporteService.ProcesarFlujoCajaPorMesAsync(idproyecto);  
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                
                using (var package = new ExcelPackage())
                {
                   
                    var worksheet = package.Workbook.Worksheets.Add("Flujo de Caja");

                    var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "unitt.png");
                    var image = new FileInfo(logoPath);
                    if (image.Exists)
                    {

                        var picture = worksheet.Drawings.AddPicture("Logo", image);
                        picture.SetPosition(0, 0, 0, 0);

                    }
                    worksheet.Cells[3, 5].Value = "Flujo de Caja Proyectos Unit";
                    worksheet.Cells[3, 5, 4, 13].Merge = true;
                    worksheet.Cells[3, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Centrar el texto
                    worksheet.Cells[3, 5].Style.Font.Bold = true;
                    worksheet.Cells[3, 5].Style.Font.Size = 15;

                    
                    string[] headers = { "Proyecto", "Tipo", "Cuenta" };
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        worksheet.Cells[5, col].Value = headers[col - 1];

                        
                        worksheet.Cells[5, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[5, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        
                        worksheet.Cells[5, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[5, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    
                    var mesesAnios = new HashSet<(int, int)>();
                    foreach (var proyecto in model)
                    {
                        foreach (var tipo in proyecto.Value)
                        {
                            foreach (var mesAnio in tipo.Value.Keys)
                            {
                                mesesAnios.Add(mesAnio);
                            }
                        }
                    }

                  
                    var mesesAniosOrdenados = mesesAnios.OrderBy(ma => ma.Item2).ThenBy(ma => ma.Item1).ToList();
                    var columnaPorMesAnio = new Dictionary<(int, int), int>();
                   
                    int columnaInicio = 4;  
                    foreach (var mesAnio in mesesAniosOrdenados)
                    {
                      
                        string mesAnioFormateado = $"{mesAnio.Item1:D2}-{mesAnio.Item2:D2}";
                        worksheet.Cells[5, columnaInicio].Value = mesAnioFormateado;
                        worksheet.Cells[5, columnaInicio].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[5, columnaInicio].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[5, columnaInicio].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[5, columnaInicio].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        worksheet.Cells[5, columnaInicio, 5, columnaInicio + 2].Merge = true; 

                        
                        worksheet.Cells[6, columnaInicio].Value = "Proyectado";
                        worksheet.Cells[6, columnaInicio].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[6, columnaInicio].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 1].Value = "Real";
                        worksheet.Cells[6, columnaInicio + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 2].Value = "Forecast";
                        worksheet.Cells[6, columnaInicio + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                        worksheet.Cells[6, columnaInicio].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[6, columnaInicio].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[6, columnaInicio + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[6, columnaInicio + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[6, columnaInicio + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[6, columnaInicio + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                     
                        worksheet.Cells[6, columnaInicio].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[6, columnaInicio].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[6, columnaInicio + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        columnaPorMesAnio[mesAnio] = columnaInicio;
                        columnaInicio += 3;  
                    }

                   
                    int fila = 7;

                    
                    foreach (var proyecto in model.OrderBy(p => p.Key))
                    {
                        /*egresos*/
                        var totalesProyectado = new Dictionary<(int, int), decimal>();
                        var totalesReal = new Dictionary<(int, int), decimal>();
                        var totalesForecast = new Dictionary<(int, int), decimal>();
                        /*ingresos*/
                        var totalIngresoProyectadoPorMes = new Dictionary<(int , int), decimal>();
                        var totalIngresoRealPorMes = new Dictionary<(int , int ), decimal>();
                        foreach (var tipo in proyecto.Value)
                        {
                            
                            int columna = 1;
                            worksheet.Cells[fila, columna++].Value = proyecto.Key;  
                            worksheet.Cells[fila, columna++].Value = tipo.Key;       

                            var primeraCuenta = tipo.Value.Values
                                .SelectMany(flujoList => flujoList)
                                .FirstOrDefault();

                            var cuenta = primeraCuenta != null ? primeraCuenta.Cuenta : "No disponible";
                            worksheet.Cells[fila, columna++].Value = cuenta;       

                       
                            foreach (var mesAnio in mesesAniosOrdenados)
                            {
                                var flujo = tipo.Value
                                    .FirstOrDefault(flujoList =>
                                        flujoList.Key.Item1 == mesAnio.Item1 && flujoList.Key.Item2 == mesAnio.Item2);

                                decimal proyectado = 0;
                                decimal real = 0;
                                decimal forecast = 0;

                                if (flujo.Key != default)
                                {
                                    proyectado = flujo.Value
                                        .Where(f => f.Estado == "Proyectado")
                                        .Sum(f => f.Monto);

                                    real = flujo.Value
                                        .Where(f => f.Estado == "Real")
                                        .Sum(f => f.Monto);

                                    forecast = flujo.Value
                                        .Where(f => f.Estado == "Forecast")
                                        .Sum(f => f.Monto);
                                }

                           
                                var columnaMesAnio = columnaPorMesAnio[mesAnio];
                             
                                if (proyectado != 0)
                                {
                                    worksheet.Cells[fila, columna++].Value = proyectado;
                                    worksheet.Cells[fila, columna - 1].Style.Numberformat.Format = "#,##0";  
                                    worksheet.Cells[fila, columna - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; 
                                }
                                else
                                {
                                    worksheet.Cells[fila, columna++].Value = "";
                                }

                                if (real != 0)
                                {
                                    worksheet.Cells[fila, columna++].Value = real;
                                    worksheet.Cells[fila, columna - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columna - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                }
                                else
                                {
                                    worksheet.Cells[fila, columna++].Value = "";
                                }

                                if (forecast != 0)
                                {
                                    worksheet.Cells[fila, columna++].Value = forecast;
                                    worksheet.Cells[fila, columna - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columna - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                    worksheet.Cells[fila, columna - 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                                else
                                {
                                    worksheet.Cells[fila, columna++].Value = "";
                                }
                                if (tipo.Key != "Ingreso")
                                {
                                    {
                                        if (!totalesProyectado.ContainsKey(mesAnio)) totalesProyectado[mesAnio] = 0; 
                                        if (!totalesReal.ContainsKey(mesAnio)) totalesReal[mesAnio] = 0; 
                                        if (!totalesForecast.ContainsKey(mesAnio)) totalesForecast[mesAnio] = 0; 
                                        totalesProyectado[mesAnio] += proyectado; 
                                        totalesReal[mesAnio] += real; 
                                        totalesForecast[mesAnio] += forecast;


                                    }

                                }
                                if (tipo.Key == "Ingreso")
                                {
                                    {
                                        if (!totalIngresoProyectadoPorMes.ContainsKey(mesAnio)) totalIngresoProyectadoPorMes[mesAnio] = 0;
                                        if (!totalIngresoRealPorMes.ContainsKey(mesAnio)) totalIngresoRealPorMes[mesAnio] = 0;
                                       
                                        totalIngresoProyectadoPorMes[mesAnio] += proyectado;
                                        totalIngresoRealPorMes[mesAnio] += real;
                                       


                                    }

                                }

                            }
                            fila++;
                        }
                        /*totales egresos*/
                        worksheet.Cells[fila, 1].Value = $"Total Egreso del Proyecto {proyecto.Key}";
                        worksheet.Cells[fila, 1].Style.Font.Bold = true;

               
                        worksheet.Cells[fila, 1, fila, 3].Merge = true;

                       
                        int columnaInicioegreso = 1; 
                        int columnaFinegreso = columnaPorMesAnio.Values.Max() + 3; 

                        worksheet.Cells[fila, columnaInicioegreso, fila, columnaFinegreso].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[fila, columnaInicioegreso, fila, columnaFinegreso].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MistyRose);
                        foreach (var mesAnio in mesesAniosOrdenados) { 
                            if (columnaPorMesAnio.ContainsKey(mesAnio)) 
                            { 
                                var columnaTotales = columnaPorMesAnio[mesAnio];
                                
                                if (totalesProyectado.ContainsKey(mesAnio) && totalesProyectado[mesAnio] != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = totalesProyectado[mesAnio];
                                    worksheet.Cells[fila, columnaTotales - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columnaTotales - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = "";
                                }
                                
                                if (totalesReal.ContainsKey(mesAnio) && totalesReal[mesAnio] != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = totalesReal[mesAnio];
                                    worksheet.Cells[fila, columnaTotales - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columnaTotales - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = "";
                                }
                                
                                if (totalesForecast.ContainsKey(mesAnio) && totalesForecast[mesAnio] != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales].Value = totalesForecast[mesAnio];
                                    worksheet.Cells[fila, columnaTotales].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columnaTotales].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                    worksheet.Cells[fila, columnaTotales].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales].Value = "";
                                }
                                
                                
                            } 
                        }
                        fila++;
                        /*margen del proyecto*/
                        worksheet.Cells[fila, 1].Value = $" Margen del Proyecto {proyecto.Key}";
                        worksheet.Cells[fila, 1].Style.Font.Bold = true; worksheet.Cells[fila, 1, fila, 3].Merge = true;
                        int columnaIniciomargen = 1;
                        int columnaFinmargen = columnaPorMesAnio.Values.Max() + 3;

                        worksheet.Cells[fila, columnaIniciomargen, fila, columnaFinmargen].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[fila, columnaIniciomargen, fila, columnaFinmargen].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        foreach (var mesAnio in mesesAniosOrdenados)
                        {
                            if (columnaPorMesAnio.ContainsKey(mesAnio))
                            {
                                var columnaTotales = columnaPorMesAnio[mesAnio];
                                /*margen proyectado*/
                                var egresosproyectados = totalesProyectado.GetValueOrDefault(mesAnio, 0);
                                var ingresosproyectados = totalIngresoProyectadoPorMes.GetValueOrDefault(mesAnio, 0);
                                var margenproyectado = ingresosproyectados - egresosproyectados;
                                /*margen real*/
                                var egresosreales = totalesReal.GetValueOrDefault(mesAnio,0) ;
                                var ingresosreales = totalIngresoRealPorMes.GetValueOrDefault(mesAnio, 0);
                                var margenreal = ingresosreales - egresosreales;
                                if (margenproyectado != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = margenproyectado;
                                    worksheet.Cells[fila, columnaTotales - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columnaTotales - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = "";
                                }

                              
                                if (margenreal != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = margenreal;
                                    worksheet.Cells[fila, columnaTotales - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columnaTotales - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = "";
                                }
                                worksheet.Cells[fila, columnaTotales].Value =  "";

                            }
                        }
                        fila++;
                        /*% Margen del proyecto*/
                        worksheet.Cells[fila, 1].Value = $" Margen del Proyecto % {proyecto.Key}";
                        worksheet.Cells[fila, 1].Style.Font.Bold = true; worksheet.Cells[fila, 1, fila, 3].Merge = true;
                        int columnaIniciomargenp = 1;
                        int columnaFinmargenp = columnaPorMesAnio.Values.Max() + 3;

                        worksheet.Cells[fila, columnaIniciomargenp, fila, columnaFinmargenp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[fila, columnaIniciomargenp, fila, columnaFinmargenp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        foreach (var mesAnio in mesesAniosOrdenados)
                        {
                            if (columnaPorMesAnio.ContainsKey(mesAnio))
                            {
                                var columnaTotales = columnaPorMesAnio[mesAnio];
                               
                                /*margen real*/
                                var egresosreales = totalesReal.GetValueOrDefault(mesAnio, 0);
                                var ingresosreales = totalIngresoRealPorMes.GetValueOrDefault(mesAnio, 0);
                                var margenreal = ingresosreales - egresosreales;
                                var porcentajeMargenReal = ingresosreales > 0 ? Math.Round(margenreal / ingresosreales, 2) : 0;
                                worksheet.Cells[fila, columnaTotales++].Value = "";
                                
                                if (porcentajeMargenReal != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = porcentajeMargenReal;
                                    worksheet.Cells[fila, columnaTotales - 1].Style.Numberformat.Format = "#0%";  
                                    worksheet.Cells[fila, columnaTotales - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = "";
                                }
                                worksheet.Cells[fila, columnaTotales].Value = "";

                            }
                        }
                        fila++;
                        /*Saldo Acumulado*/
                        worksheet.Cells[fila, 1].Value = $" Saldo Acumulado {proyecto.Key}";
                        worksheet.Cells[fila, 1].Style.Font.Bold = true; worksheet.Cells[fila, 1, fila, 3].Merge = true;
                        int columnaIniciosaldo = 1;
                        int columnaFinsaldo = columnaPorMesAnio.Values.Max() + 3;

                        worksheet.Cells[fila, columnaIniciosaldo, fila, columnaFinsaldo].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[fila, columnaIniciosaldo, fila, columnaFinsaldo].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                        decimal saldoAcumulado = 0;
                        bool proyectoComienza = false;
                        bool detenerAcumulacion = false;
                        foreach (var mesAnio in mesesAniosOrdenados)
                        {
                            if (columnaPorMesAnio.ContainsKey(mesAnio))
                            {
                                var columnaTotales = columnaPorMesAnio[mesAnio];

                                /*margen real*/
                                var egresosreales = totalesReal.GetValueOrDefault(mesAnio, 0);
                                var ingresosreales = totalIngresoRealPorMes.GetValueOrDefault(mesAnio, 0);
                                var margenreal = ingresosreales - egresosreales;
                                if (!proyectoComienza && (ingresosreales != 0 || egresosreales != 0 || margenreal != 0))
                                {
                                    proyectoComienza = true;
                                }


                                if (proyectoComienza)
                                {
                                    saldoAcumulado += margenreal;

                                    if (ingresosreales == 0 && egresosreales == 0 && margenreal == 0)
                                    {
                                        detenerAcumulacion = true;
                                    }
                                }

                                if (detenerAcumulacion)
                                {
                                    break;
                                }
                                
                                worksheet.Cells[fila, columnaTotales++].Value = "";
                                if (saldoAcumulado != 0)
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = saldoAcumulado;
                                    worksheet.Cells[fila, columnaTotales - 1].Style.Numberformat.Format = "#,##0";  // Formato con puntos en los miles
                                    worksheet.Cells[fila, columnaTotales - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;  // Alineado a la derecha
                                }
                                else
                                {
                                    worksheet.Cells[fila, columnaTotales++].Value = "";
                                }
                                worksheet.Cells[fila, columnaTotales].Value = "";

                            }
                        }
                        fila++;
                    }
                    worksheet.View.FreezePanes(7, 3);
                    
                    // Aplicar formato a las celdas (opcional)
                    worksheet.Cells.AutoFitColumns();

                    // Devuelve el archivo Excel
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FlujoDeCaja.xlsx");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al generar el excel:{e.Message}");
                return View();
            }
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
                worksheet.Cells[5, 10].Value = "Moneda de Origen";
                worksheet.Cells[5, 11].Value = "Monto Moneda Origen";
                worksheet.Cells[5, 12].Value = "Tasa de Cambio";
                worksheet.Cells[5, 13].Value = "MontoCLP";
                worksheet.Cells[5, 14].Value = "Plazo";
                worksheet.Cells[5, 15].Value = "Fecha Inicio";
                worksheet.Cells[5, 16].Value = "Fecha Término";
                worksheet.Cells[5, 17].Value = "HH_Socios";
                worksheet.Cells[5, 18].Value = "HH_Staff";
                worksheet.Cells[5, 19].Value = "HH_ConsultorA";
                worksheet.Cells[5, 20].Value = "HH_ConsultorB";
                worksheet.Cells[5, 21].Value = "HH_ConsultorC";
                worksheet.Cells[5, 22].Value = "CostoSocios";
                worksheet.Cells[5, 23].Value = "CostoStaff";
                worksheet.Cells[5, 24].Value = "CostoConsultorA";
                worksheet.Cells[5, 25].Value = "CostoConsultorB";
                worksheet.Cells[5, 26].Value = "CostoConsultorC";
                worksheet.Cells[5, 27].Value = "TotalServicios";
                worksheet.Cells[5, 28].Value = "TotalGastos";
                worksheet.Cells[5, 29].Value = "Status";

                using (var range = worksheet.Cells[5, 1, 5, 29])
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
                worksheet.Column(11).Width = 27;
                worksheet.Column(12).Width = 20;
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
                worksheet.Column(27).Width = 15;
                worksheet.Column(28).Width = 15;
                worksheet.Column(29).Width = 15;
                

              
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
                    worksheet.Cells[indice, 10].Value = planilla.Moneda;
                    worksheet.Cells[indice, 11].Value = planilla.MontoMonedaInternacional;
                    worksheet.Cells[indice, 11].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 12].Value = planilla.TasaCambio;
                    worksheet.Cells[indice, 12].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 13].Value = planilla.Monto == 0 ? (object)null : planilla.Monto;
                    worksheet.Cells[indice, 13].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 14].Value = planilla.Plazo;
                    worksheet.Cells[indice, 15].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[indice, 15].Value = planilla.Fechainicio?.ToString("dd/MM/yyyy");
                    worksheet.Cells[indice, 16].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[indice, 16].Value = planilla.Fechatermino?.ToString("dd/MM/yyyy");
                    worksheet.Cells[indice, 17].Value = planilla.HHsocios == 0 ? (object)null : planilla.HHsocios;
                    worksheet.Cells[indice, 18].Value = planilla.HHstaff == 0 ? (object)null : planilla.HHstaff;
                    worksheet.Cells[indice, 19].Value = planilla.HHConsultorA == 0 ? (object)null : planilla.HHConsultorA;
                    worksheet.Cells[indice, 20].Value = planilla.HHConsultorB == 0 ? (object)null : planilla.HHConsultorB;
                    worksheet.Cells[indice, 21].Value = planilla.HHConsultorC == 0 ? (object)null : planilla.HHConsultorC;
                    worksheet.Cells[indice, 22].Value = planilla.Costosocios == 0 ? (object)null : planilla.Costosocios;
                    worksheet.Cells[indice, 22].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 23].Value = planilla.Costostaff == 0 ? (object)null : planilla.Costostaff;
                    worksheet.Cells[indice, 23].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 24].Value = planilla.CostoconsultorA == 0 ? (object)null : planilla.CostoconsultorA;
                    worksheet.Cells[indice, 24].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 25].Value = planilla.CostoconsultorB == 0 ? (object)null : planilla.CostoconsultorB;
                    worksheet.Cells[indice, 25].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 26].Value = planilla.CostoconsultorC == 0 ? (object)null : planilla.CostoconsultorC;
                    worksheet.Cells[indice, 26].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 27].Value = planilla.TotalServicios == 0 ? (object)null : planilla.TotalServicios;
                    worksheet.Cells[indice, 27].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[indice, 28].Value = planilla.TotalGastos == 0 ? (object)null : planilla.TotalGastos;
                    worksheet.Cells[indice, 28].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[indice, 29].Value = planilla.Status;
                    
                }
                worksheet.View.FreezePanes(6, 3);
                string nombreArchivo = $"Base_Datos_Proyectos.xlsx";
                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
            }
        }
    }
}
