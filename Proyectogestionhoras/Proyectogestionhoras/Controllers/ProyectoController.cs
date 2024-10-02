using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
namespace Proyectogestionhoras.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly ClienteService clienteService;
        private readonly UsuarioService usuarioService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public ProyectoController(ProyectoService proyectoService,ClienteService clienteService,UsuarioService usuarioService,PROYECTO_CONTROL_HORASContext context)
        {
            this.proyectoService = proyectoService;
            this.clienteService = clienteService;
            this.usuarioService = usuarioService;
            this.context = context;
        } 
        public async Task<IActionResult> NuevoProyecto()
        {
            var unegocios = await proyectoService.ObtenerUnegocio();
            var ccostos = await proyectoService.ObtenerCcosto();
            var empresas = await proyectoService.ObtenerEmpresa();
            var tipologias = await proyectoService.ObtenerTipoligias();
            var clientes = await clienteService.ObtenerClientesIndex(0);
            var status = await proyectoService.ObtenerStatus();
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


        [HttpPost]
        public async Task<IActionResult> CrearProyecto(decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, string numproyecto, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int idclientesucursal, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg)
        {
            try
            {
                int idcosto = int.Parse(Request.Form["centroCosto"]);
                int idunegocio = int.Parse(Request.Form["unidadNegocio"]);
                int idcliente = int.Parse(Request.Form["cliente"]);
                int idsucursal = int.Parse(Request.Form["sucursal"]);
                int idsucursalcliente = await GetIdClienteSucrusal(idcliente, idsucursal);
                int idcodigoccosto = await GetCostoUNegocioIdAsync(idcosto, idunegocio);
                bool resultado = await proyectoService.CrearProyecto(monto,moneda,afectaiva,idtipologia,nombre,numproyecto,fechainicio,fechatermino,plazo,tipoempresa,idcodigoccosto,idsucursalcliente,status,probabilidad,porcentajeprobabilidad,fechaplazoneg);
                if (resultado)
                {
                    return RedirectToAction("ExitoCreacion");
                }
                else
                {
                    return View();
                }
                
            }
            catch (Exception ex) {

                Debug.WriteLine($"Hubo un error al registrar el proyecto:{ex.Message}");

                return View();
            }
        }

        public IActionResult ExitoCreacion()
        {
            return View();
        }
        public async Task<int> GetIdClienteSucrusal(int idcliente, int idsucursal)
        {
            return await context.SucursalClientes
                .Where(sc => sc.IdCliente == idcliente && sc.IdSucursal == idsucursal)
                .Select(sc => sc.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetCostoUNegocioIdAsync(int idCosto, int idUnidadNegocio)
        {
            return await context.CcostoUnegocios
                .Where(cu => cu.IdCcosto == idCosto && cu.IdUnegocio == idUnidadNegocio)
                .Select(cu => cu.Id)
                .FirstOrDefaultAsync();
        }


        [HttpGet]
        public IActionResult ObtenerUltimoNumProyecto()
        {
            var ultimoproyecto = context.Proyectos.OrderByDescending(p => p.Id).Select(p => p.NumProyecto).FirstOrDefault();
            string nuevonumproyecto = GenerarNuevoNumeroProyecto(ultimoproyecto);
            return Json(nuevonumproyecto);
        }

        public string GenerarNuevoNumeroProyecto(string ultimoproyecto)
        {
            string nuevonumproyecto = string.Empty;
            int anioactual = DateTime.Now.Year;
            if (!string.IsNullOrEmpty(ultimoproyecto))
            {
                var partes = ultimoproyecto.Split('/');
                int anioultimoproyecto = int.Parse(partes[0]);
                int correlativo = int.Parse(partes[1]);
                if(anioultimoproyecto == anioactual)
                {
                    correlativo++;
                    nuevonumproyecto = $"{anioactual}/{correlativo:D2}";
                }
                else
                {
                    nuevonumproyecto = $"{anioactual}/01";
                }

            }
            else
            {
                nuevonumproyecto = $"{anioactual}/01";
            }
            return nuevonumproyecto;
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
