using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> ReportesFactura()
        {
            var facturas = await _reporteService.RecuperarFacturasProyectos();
            return View(facturas);
        }
    }
}
