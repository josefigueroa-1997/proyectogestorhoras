using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Proyectogestionhoras.Controllers
{
    public class PropuestaController : Controller
    {


        private readonly ProyectoService proyectoService;
        private readonly FacturaService facturaService;
        private readonly EjecucionService ejecucionService;
        private readonly ReporteService reporteService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public PropuestaController(ProyectoService proyectoService, FacturaService facturaService, EjecucionService ejecucionService, PROYECTO_CONTROL_HORASContext context, ReporteService reporteService)
        {
            this.proyectoService = proyectoService;
            this.facturaService = facturaService;
            this.ejecucionService = ejecucionService;
            this.context = context;
            this.reporteService = reporteService;
        }





        [HttpGet]
        public async Task<IActionResult> CargarGastosReales(int idproyecto, int pagina = 1, int tamanioPagina = 50)
        {
            var resultadoCompleto = await ejecucionService.ObtenerGastosReales(idproyecto);

            var total = resultadoCompleto.Count;
            var datos = resultadoCompleto
                            .Skip((pagina - 1) * tamanioPagina)
                            .Take(tamanioPagina)
                            .ToList();

            return Json(new { datos, total });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarGastos([FromBody] GuardarGastosRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Request es null. Verifica el formato de los datos enviados."
                });
            }

            try
            {
                if (request.gastos == null)
                {
                    return Json(new { success = false, message = "La lista de gastos es null" });
                }

                await ejecucionService.GestorGastosReales(request.idproyecto, request.gastos);
                return Json(new { success = true, message = "Éxito en el registro/actualización" });
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error al guardar gastos: {ex.ToString()}");
                return Json(new
                {
                    success = false,
                    message = "Error interno al procesar la solicitud",
                    error = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> CargarServiciosReales(int idproyecto, string tipo)
        {
            var resultado = await ejecucionService.ObtenerServiciosReales(idproyecto, tipo);

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosResumen(int idProyecto)
        {
            try
            {
                //hh socios

                decimal? costohhsocio = await context.HistorialCostosProyectos
                    .Where(hc => hc.Idproyecto == idProyecto)
                    .Select(hc => hc.Costosocio)
                    .FirstOrDefaultAsync();

                decimal? hhsocioinicio = await context.HhUsuarioHistorials.Where(h => h.IdProyecto == idProyecto).Select(h => h.HhSocios).FirstOrDefaultAsync();

                decimal? hhsocioproyectadas = costohhsocio * hhsocioinicio;

                decimal? hhsociosreales = await context.Gastoshhhejecucions.Where(context => context.Idproyecto == idProyecto && context.Tiporecurso == "Socio" && context.Estado == 1)
                    .SumAsync(context => context.Monto);

                decimal? hhsociosforecast = await context.Gastoshhhejecucions.Where(context => context.Idproyecto == idProyecto && context.Tiporecurso == "Socio" && context.Estado == 2)
                    .SumAsync(context => context.Monto);

                //hh staff


                decimal? costohhstaff = await context.HistorialCostosProyectos
                    .Where(hc => hc.Idproyecto == idProyecto)
                    .Select(hc => hc.Costostaff)
                    .FirstOrDefaultAsync();

                decimal? hhstaffinicio = await context.HhUsuarioHistorials.Where(h => h.IdProyecto == idProyecto).Select(h => h.HhStaff).FirstOrDefaultAsync();

                decimal? hhstaffproyectadas = costohhstaff * hhstaffinicio;


                decimal? hhstaffreales = await context.Gastoshhhejecucions.Where(context => context.Idproyecto == idProyecto && context.Tiporecurso == "Staff" && context.Estado == 1)
                    .SumAsync(context => context.Monto);

                decimal? hhstaffforecast = await context.Gastoshhhejecucions.Where(context => context.Idproyecto == idProyecto && context.Tiporecurso == "Staff" && context.Estado == 2)
                    .SumAsync(context => context.Monto);


                var serviciosProyecto = await proyectoService.ObtenerServiciosProyecto(idProyecto);
                var gastosProyecto = await proyectoService.ObtenerGastosProyectos(idProyecto);


                var serviciosejecucion = await context.Serviciosejecucions
                    .Where(s => s.Idproyecto == idProyecto)
                    .ToListAsync();

                var gastosejecucion = await context.Gastosejecucions
                    .Where(s => s.Idproyecto == idProyecto)
                    .ToListAsync();


                var serviciosreales = serviciosejecucion
                    .GroupBy(x => new { x.Idservicio, x.Estado })
                    .Select(g => new
                    {
                        Idservicio = g.Key.Idservicio,
                        Estado = g.Key.Estado,
                        TotalMonto = g.Sum(x => x.Monto)
                    }).ToList();

                var gastosreales = gastosejecucion
                    .GroupBy(x => new { x.Idgasto, x.Estado })
                    .Select(g => new
                    {
                        Idgasto = g.Key.Idgasto,
                        Estado = g.Key.Estado,
                        TotalMonto = g.Sum(x => x.Monto)
                    }).ToList();

                // Diccionarios de totales
                var serviciosTotalesPagados = serviciosreales
                    .Where(x => x.Estado == "Pagada")
                    .GroupBy(x => x.Idservicio.Value)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var serviciosTotalesForecast = serviciosreales
                    .Where(x => x.Estado == "Forecast")
                    .GroupBy(x => x.Idservicio.Value)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var gastosTotalesPagados = gastosreales
                    .Where(x => x.Estado == "Pagada")
                    .GroupBy(x => x.Idgasto.Value)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var gastosTotalesForecast = gastosreales
                    .Where(x => x.Estado == "Forecast")
                    .GroupBy(x => x.Idgasto.Value)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var jsonResult = JsonConvert.SerializeObject(new
                {
                    ServiciosProyectos = serviciosProyecto,
                    ServiciosTotalesPagados = serviciosTotalesPagados,
                    ServiciosTotalesForecast = serviciosTotalesForecast,
                    GastosProyectos = gastosProyecto,
                    GastosTotalesPagados = gastosTotalesPagados,
                    GastosTotalesForecast = gastosTotalesForecast,
                    HHSociosProyectadas = hhsocioproyectadas,
                    HHSociosReales = hhsociosreales,
                    HHsociosForecast = hhsociosforecast,
                    HHStaffProyectadas = hhstaffproyectadas,
                    HHStaffReales = hhstaffreales,
                    HHStaffForecast = hhstaffforecast
                });

                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR EJECUCION: " + ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }





        public async Task<IActionResult> GestorProyectos(int? idproyecto, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var idsession = HttpContext.Session.GetInt32("id");
            if (idsession.HasValue)
            {
                ViewBag.AnchoExtendido = true;
                Debug.WriteLine(idproyecto);
                var proyecto = await proyectoService.ObtenerProyectos(idproyecto, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
                ViewBag.Proyecto = proyecto;

                var gastoshh = await ejecucionService.ObtenerDistribucionHH(idproyecto, null);
                ViewBag.Idcuentasocio = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Idcuentasocio).FirstOrDefaultAsync();
                ViewBag.cuentasocio = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Cuentasocio).FirstOrDefaultAsync();
                ViewBag.costohhsocio = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Costosocio).FirstOrDefaultAsync();
                ViewBag.Idcuentastaff = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Idcuentastaff).FirstOrDefaultAsync();
                ViewBag.cuentastaff = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Cuentastaff).FirstOrDefaultAsync();
                ViewBag.costohhstaff = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Costostaff).FirstOrDefaultAsync();
                var datosgastosrecursos = await context.Gastoshhhejecucions.Where(g => g.Idproyecto == idproyecto).ToListAsync();
                ViewBag.GastosHH = gastoshh;
                ViewBag.GastosRecursos = datosgastosrecursos;
                var flujocaja = await reporteService.ProcesarFlujoCajaPorMesAsync(idproyecto);
                return View(flujocaja);

            }

            return RedirectToAction("Index", "Home");

        }

    }
}
