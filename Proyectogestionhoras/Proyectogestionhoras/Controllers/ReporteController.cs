using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;

namespace Proyectogestionhoras.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ReporteService _reporteService;
        public ReporteController(ReporteService reporteService)
        {
            _reporteService = reporteService;
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
    }
}
