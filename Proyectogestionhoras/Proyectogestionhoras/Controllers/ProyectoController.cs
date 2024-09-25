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
            var status = await proyectoService.ObtenerStatus(0);
            ViewBag.Clientes = clientes;
            ViewBag.Tipologias = tipologias;
            ViewBag.Empresas = empresas;
            ViewBag.Ccostos = ccostos;
            ViewBag.Unegocios = unegocios;
            ViewBag.Status = status;
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Getcodigoccosto(int idcosto,int idunegocio)
        {
            var codigo = await proyectoService.ObtenerCodigoCCosto(idcosto,idunegocio);
            return Json(codigo);
        }

        [HttpGet]
        public async Task<IActionResult> GetValoresFactura(int idcosto,int idunegocio)
        {
            var factura = await proyectoService.ObtenerValoresFactura(idcosto, idunegocio);
            return Ok(factura);
        }
        [HttpGet]
        public async Task<IActionResult> GetValoresServicios(int idcodigo)
        {
            var servicio = await proyectoService.ObtenerValoresServicios(idcodigo);
            return Ok(servicio);
        }

        public IActionResult GetProyectos()
        {
            return View("Proyectos");
        }

        public IActionResult GetProyectosUnidadNegocio()
        {
            return View("UnidadNegocio");
        }
    }
}
