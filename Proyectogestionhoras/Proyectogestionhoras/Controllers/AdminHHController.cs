using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;

using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Services;
namespace Proyectogestionhoras.Controllers
{
    public class AdminHHController : Controller
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly ReporteService reporteService;
        public AdminHHController(PROYECTO_CONTROL_HORASContext context, ReporteService reporteService)
        {

            this.context = context;
            this.reporteService = reporteService;
        }
        public async Task<IActionResult> SeleccionarMesAnioPlanillaAdmin()
        {
            var id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {

                if (id == 88 || id == 96) {


                    var planillas = await context.Planillas
             .OrderBy(p => p.Anio)
             .ThenBy(p => p.Mes)
             .ToListAsync();

                    var planillasAgrupadas = planillas
                        .GroupBy(p => p.Anio)
                        .Select(g => new
                        {
                            Anio = g.Key,
                            Meses = g.DistinctBy(p => p.Mes)
                                .Select(p => new { p.Id, p.Mes })
                                 .ToList()
                        })
                        .ToList();

                    ViewBag.PlanillasPorAnio = planillasAgrupadas;
                    return View();





                } 
            
            
            }
            return RedirectToAction("Login", "Usuario");


        }


        public async Task<IActionResult> PlanillaEditar(int? mes, int? anio)
        {
            var id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {

                if (id == 88 || id == 96)
                {
                    var registros = await reporteService.ReporteHHGeneral(mes, anio);
                    var subactividades = await context.Subactividads.Select(s => new { s.Id, s.Nombre, s.Idactividad }).ToListAsync();
                    var actividad = await context.Actividades.ToListAsync();
                    var ejecucion = await context.Proyectos.Where(p => p.StatusProyecto == 2).Select(p => new { p.Id, p.Nombre }).ToListAsync();
                    var negociacion = await context.Proyectos.Where(p => p.StatusProyecto == 1).Select(p => new { p.Id, p.Nombre, p.StatusProyecto }).ToListAsync();
                    ViewBag.Subactividades = subactividades;
                    ViewBag.Negociacion = negociacion;
                    ViewBag.Actividad = actividad;
                    ViewBag.Ejecucion = ejecucion;
                    ViewBag.Registros = registros;
                    ViewBag.MesSeleccionado = mes;
                    ViewBag.AnioSeleccionado = anio;

                    return View();




                }
            }
            return RedirectToAction("Login","Usuario");
        }
    }
}
