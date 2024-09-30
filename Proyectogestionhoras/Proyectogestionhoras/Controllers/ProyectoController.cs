using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Services;
namespace Proyectogestionhoras.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly ClienteService clienteService;
        private readonly UsuarioService usuarioService;
        
        public ProyectoController(ProyectoService proyectoService,ClienteService clienteService,UsuarioService usuarioService)
        {
            this.proyectoService = proyectoService;
            this.clienteService = clienteService;
            this.usuarioService = usuarioService;
        } 
        public async Task<IActionResult> NuevoProyecto()
        {
            var unegocios = await proyectoService.ObtenerUnegocio();
            var ccostos = await proyectoService.ObtenerCcosto();
            var empresas = await proyectoService.ObtenerEmpresa();
            var tipologias = await proyectoService.ObtenerTipoligias();
            var clientes = await clienteService.ObtenerClientesIndex(0);
            var status = await proyectoService.ObtenerStatus(0);
            var recursos = await usuarioService.ObtenerUusario(0,null,0);
            ViewBag.Recursos = recursos;
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

        [HttpGet]
        public async Task<IActionResult> GetSegmentosGastos(int idcodigo)
        {
            var segmentos = await proyectoService.GetValoresSegmentos(idcodigo);
            return Ok(segmentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerValoresGastos(int idcodigo,string nombresegmento)
        {
            var valoresgastos = await proyectoService.ObtenerValoresGastos(idcodigo, nombresegmento);
            return Json(valoresgastos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerGastos()
        {
            var gastos = await proyectoService.ObtenerGastos();
            return Ok(gastos);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerServicios()
        {
            var servicios = await proyectoService.ObtenerServicios();
            return Ok(servicios);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerValoresConsultores(int idcodigo)
        {
            var consultores = await proyectoService.ObtenerValoresConsultores(idcodigo);
            return Json(consultores);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerValoresHonorarios(int idcodigo,int idrecurso)
        {
            var honorarios = await proyectoService.ObtenerValoresHonorarios(idcodigo,idrecurso);
            return Json(honorarios);
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
