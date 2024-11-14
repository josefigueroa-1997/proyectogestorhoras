using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyectogestionhoras.Controllers
{
    public class EjecucionProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly FacturaService facturaService;
        public EjecucionProyectoController(ProyectoService proyectoService, FacturaService facturaService)
        {
            this.proyectoService = proyectoService;
            this.facturaService = facturaService;
        }

        public async Task<IActionResult> SeleccionarProyecto(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            ViewBag.Proyectos = proyectos;
            return View();
        }

        [HttpPost]
        public IActionResult RedirigirActividad()
        {
            int idproyecto = int.Parse(Request.Form["idproyecto"].ToString());
            HttpContext.Session.SetInt32("numproyecto", idproyecto);
            return RedirectToAction("SeleccionarActividad", "EjecucionProyecto");
        }
        public IActionResult SeleccionarActividad() {

            return View();
        }
        public async Task<IActionResult> ForecastIngreso(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            var factura = await facturaService.RecuperarFacturas(id);
            ViewBag.Proyecto = proyecto;
            ViewBag.Factura = factura;
            return View();
        }

        public async Task<IActionResult> ForecastCostos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            ViewBag.Proyecto = proyecto;
            return View();
        }
    }
}
