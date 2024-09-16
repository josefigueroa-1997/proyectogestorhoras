using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Services;
namespace Proyectogestionhoras.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly ClienteService clienteService;
        
        public ProyectoController(ProyectoService proyectoService,ClienteService clienteService)
        {
            this.proyectoService = proyectoService;
            this.clienteService = clienteService;
        } 
        public async Task<IActionResult> NuevoProyecto()
        {
            var unegocios = await proyectoService.ObtenerUnegocio();
            var ccostos = await proyectoService.ObtenerCcosto();
            var empresas = await proyectoService.ObtenerEmpresa();
            var tipologias = await proyectoService.ObtenerTipoligias();
            var clientes = await clienteService.ObtenerClientesIndex(0);
            ViewBag.Clientes = clientes;
            ViewBag.Tipologias = tipologias;
            ViewBag.Empresas = empresas;
            ViewBag.Ccostos = ccostos;
            ViewBag.Unegocios = unegocios;
            return View();
        }
    }
}
