using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using System.Reflection.Metadata.Ecma335;

namespace Proyectogestionhoras.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ReporteService _reporteService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public ReporteController(ReporteService reporteService,PROYECTO_CONTROL_HORASContext context)
        {
            _reporteService = reporteService;
            this.context = context;
        }

        public IActionResult TodosReportes()
        {
            return View();
        }
        public async Task<IActionResult> ReportesFactura()
        {
            var facturas = await _reporteService.RecuperarFacturasProyectos();
            return View(facturas);
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
        public async Task<IActionResult> ReporteResumenQuarter()
        {
            var quarter = await _reporteService.RecuperarQuarter();
            ViewBag.Quarter = quarter;
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
            var quarter = await _reporteService.RecuperarQuarter();
            var quarteranio = await _reporteService.RecuperarQuarterAnio(anio);
            ViewBag.Quarter = quarter;
            ViewBag.QuarterAnio = quarteranio;
            return View();
        }

        public async Task<IActionResult> ReporteNeogiciacionKey()
        {
            var reporte = await _reporteService.ReporteNegociacionKeys();
            ViewBag.Negociacion = reporte;
            return View();
        }
    }
}
