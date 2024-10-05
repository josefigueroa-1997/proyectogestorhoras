using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Models.DTO;
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
            var segmentoscostos = await ObtenerSegmentosCostos();
            ViewBag.SegmentoCosto = segmentoscostos;
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
        public async Task<IActionResult> CrearProyecto(decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, string numproyecto, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int idclientesucursal, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura)
        {
            try
            {
                int idcosto = int.Parse(Request.Form["centroCosto"]);
                int idunegocio = int.Parse(Request.Form["unidadNegocio"]);
                int idcliente = int.Parse(Request.Form["cliente"]);
                int idsucursal = int.Parse(Request.Form["sucursal"]);
                int idsucursalcliente = await GetIdClienteSucrusal(idcliente, idsucursal);
                int idcodigoccosto = await GetCostoUNegocioIdAsync(idcosto, idunegocio);

                List<ServicioViewModel> servicios = new List<ServicioViewModel>();
                var idsservicios = Request.Form["idservicio"];
                var montoservicio = Request.Form["montoservicio"];
                var idsegmentoservicio = Request.Form["idsegmentoservicio"];

                for (int i = 0; i < idsservicios.Count; i++)
                {
                    var servicioViewModel = new ServicioViewModel
                    {
                        Idservicios = int.Parse(idsservicios[i]),
                        IdSegmento = int.Parse(idsegmentoservicio[i]),
                        MontoServicio = decimal.Parse(montoservicio[i])
                    };

                    servicios.Add(servicioViewModel);
                }

                List<GastoViewModel> gastos = new List<GastoViewModel>();
                var idgastos = Request.Form["idgastos[]"];
                var montogasto = Request.Form["montogasto"];
                var idsegmentogasto = Request.Form["idsegmentogasto"];
                
                for (int i = 0; i < idgastos.Count; i++)
                {
                    var gastoviewmodel = new GastoViewModel
                    {
                        Idgastos = int.Parse (idgastos[i]),
                        IdSegmento = int.Parse(idsegmentogasto[i]),
                        MontoGasto = decimal.Parse (montogasto[i])
                    };
                    gastos.Add(gastoviewmodel);
                }

                bool resultado = await proyectoService.CrearProyecto(monto,moneda,afectaiva,idtipologia,nombre,numproyecto,fechainicio,fechatermino,plazo,tipoempresa,idcodigoccosto,idsucursalcliente,status,probabilidad,porcentajeprobabilidad,fechaplazoneg, hhsocios,hhstaff, hhconsultora, hhconsultorb,hhconsultorc,  idsegmentosocio,  idsegmentostaff,  idsegmentoconsultora,  idsegmentoconsultorb,  idsegmentoconsultorc,  idsegmentofactura, servicios, gastos);
                if (resultado)
                {
                    int idproyectoultimo = ultimoidproyecto();
                    return RedirectToAction("ObtenerProyectos","Proyecto",new {id= idproyectoultimo });
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

        public async Task<IActionResult> ObtenerProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto)
        {
            var proyectos = await proyectoService.ObtenerProyectos(id,idcliente,nombre,idtipoempresa,statusproyecto,numproyecto,idtipologia,unidadneg,idccosto);
            ViewBag.Proyectos = proyectos;
            return View("DetalleProyecto");
        }

        public int ultimoidproyecto()
        {
            var ultimoProyecto = context.Proyectos
                                         .OrderByDescending(p => p.Id)
                                         .FirstOrDefault();


            if (ultimoProyecto != null)
            {
                return ultimoProyecto.Id;
            }


            return 0;
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


        public async Task<List<Segmento>> ObtenerSegmentosCostos()
        {
            var segmentosCostos = await context.Segmentos
                
                .Select(s => new Segmento { Id = s.Id, Nombre = s.Nombre,TipoSegmento=s.TipoSegmento }) 
                .ToListAsync(); 

            return segmentosCostos;
        }



        [HttpGet]
        public async Task<ActionResult<List<Segmento>>> ObtenerSegmentosCostosjson()
        {
            var segmentosCostos = await context.Segmentos
                .Where(s => s.TipoSegmento == "Costos") 
                .Select(s => new Segmento { Id = s.Id, Nombre = s.Nombre })
                .ToListAsync();

            return Ok(segmentosCostos);
        }

        [HttpGet]
        public async Task<ActionResult<List<ConsultoresDTO>>> ObtenerCuentacostos(int idsegmento)
        {
           
            var resultados = await (from s in context.Segmentos
                                    join c in context.Cuenta on s.IdCuenta equals c.Id
                                    where s.Id == idsegmento
                                    select new ConsultoresDTO
                                    {
                                        
                                        IDCUENTA = c.Idcuenta,
                                        CUENTA = c.Cuenta,
                                       
                                    }).ToListAsync();

            // Retorna el resultado como JSON
            return Ok(resultados);
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
        public async Task<IActionResult> GetValoresServicios(int idcosto, int idunegocio)
        {
            var servicio = await proyectoService.ObtenerValoresServicios(idcosto,idunegocio);
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
        public async Task<IActionResult> ObtenerValoresConsultores(int idcosto, int idunegocio)
        {
            var consultores = await proyectoService.ObtenerValoresConsultores(idcosto,idunegocio);
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
