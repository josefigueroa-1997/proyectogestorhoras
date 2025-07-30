using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
namespace Proyectogestionhoras.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClienteService cliente;
        private readonly UsuarioService usuario;
        private readonly EjecucionService ejecucionService;
        private readonly PlanillaService planilla;
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly ReporteService reporteService;
        public HomeController(ILogger<HomeController> logger, ClienteService cliente, UsuarioService usuario, PlanillaService planilla, EjecucionService ejecucionService, PROYECTO_CONTROL_HORASContext context, ReporteService reporteService)
        {
            _logger = logger;
            this.cliente = cliente;
            this.usuario = usuario;
            this.planilla = planilla;
            this.ejecucionService = ejecucionService;
            this.context = context;
            this.reporteService = reporteService;
        }

        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {
                        var clientes = await cliente.ObtenerClientesIndex(0);
                        var usuariosproyectos = await usuario.ObtenerUsuariosProyecto(id.Value);
                        await usuario.GestorHHSocios();
                        await usuario.GestorHHStaff();
                       // await ejecucionService.PagoAutomaticoHH();
                        ViewBag.clientes = clientes;
                        ViewBag.Proyectos = usuariosproyectos;
                        return View();
                    }
                    else
                    {
                        var planillas = await planilla.ObtenerPlanillaUsuario(id.Value, 0);
                        ViewBag.Planilla = planillas;
                        return RedirectToAction("PlanillaRegistro", "Planilla");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");


        }

        public IActionResult Dashboard()
        {
            return View();
        }


        [HttpGet]

        public IActionResult ObtenerCantidadEstadoProyecto(int anio)
        {
            var resultado = from p in context.Proyectos
                            join s in context.StatusProyectos on p.StatusProyecto equals s.Id
                            where p.FechaInicio.Year == anio
                           group s by s.TipoStatus into g
                           select new
                           {
                               Estado = g.Key,
                               Cantidad = g.Count()
                           };
            return Json(resultado);
        }

        [HttpGet]
        public IActionResult ObtenerIngresosRealesAnio(int anio)
        {
            var ingresos = from ir in context.Ingresosreales
                           join p in context.Proyectos on ir.Idproyecto equals p.Id
                           where ir.FechaPago.HasValue && ir.FechaPago.Value.Year == anio && ir.Estado == "Pagada"
                           group ir by new { ir.FechaPago.Value.Month, ir.Idproyecto, p.Nombre } into g
                           select new
                           {
                               Mes = g.Key.Month,
                               Proyecto = g.Key.Nombre,
                               Total = g.Sum(x => x.Montoclp)
                           };

            return Json(ingresos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerEgresosPagados(int anio)
        {
            var resultado = await reporteService.ObtenereGRESOSRealesAnios(anio);
            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> Obteneringresosrealesvsforecast(int anio)
        {
            var resultado = await reporteService.ObtenerIngresosRealesvsForecast(anio);
            return Json(resultado);
        }

        [HttpGet]
        public IActionResult ObtenerDatosGantt()
        {
            var resultado = from p in context.Proyectos
                            where p.StatusProyecto == 2
                            select new
                            {
                                Id = p.Id,
                                Nombre = p.Nombre,
                                FechaInicio = p.FechaInicio,
                                FechaTermino = p.FechaTermino,
                                FechaActual = DateTime.Now
                            };
            return Json(resultado);
        }
        [HttpGet]
        public async Task<IActionResult> FiltrarProyectosEstado(string estado,int anio)
        {
            var resultado = await (from p in context.Proyectos
                                   join s in context.StatusProyectos on p.StatusProyecto equals s.Id
                                   where s.TipoStatus == estado && p.FechaInicio.Year == anio
                                   select new
                                   {
                                       p.Id,
                                       p.Nombre
                                   }).ToListAsync(); 

            return Ok(resultado); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
