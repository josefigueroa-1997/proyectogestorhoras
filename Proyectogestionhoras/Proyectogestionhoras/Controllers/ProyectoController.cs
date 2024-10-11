using Microsoft.AspNetCore.Mvc;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Text.RegularExpressions;
using Serilog;
using Microsoft.Extensions.Primitives;
namespace Proyectogestionhoras.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly ClienteService clienteService;
        private readonly UsuarioService usuarioService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly FacturaService facturaService;
        public ProyectoController(ProyectoService proyectoService,ClienteService clienteService,UsuarioService usuarioService,PROYECTO_CONTROL_HORASContext context,FacturaService facturaService)
        {
            this.proyectoService = proyectoService;
            this.clienteService = clienteService;
            this.usuarioService = usuarioService;
            this.context = context;
            this.facturaService = facturaService;
        }


        public async Task <IActionResult> GetProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyectos = await proyectoService.ObtenerProyectos(id,idcliente,nombre,idtipoempresa,statusproyecto,numproyecto,idtipologia,unidadneg,idccosto,idusuario);
            ViewBag.Proyectos = proyectos;
            return View("Proyectos");
        }



        public async Task<IActionResult> NuevoProyecto()
        {
            var unegocios = await proyectoService.ObtenerUnegocio();
            var ccostos = await proyectoService.ObtenerCcosto();
            var empresas = await proyectoService.ObtenerEmpresa();
            var tipologias = await proyectoService.ObtenerTipoligias();
            
            var status = await proyectoService.ObtenerStatus();
            var recursos = await usuarioService.ObtenerUusario(0,null,0);
            var segmentoscostos = await ObtenerSegmentosCostos();
            var clientes = await context.Clientes.ToListAsync();
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
                var montopresupuesto = Request.Form["monto"].ToString();
                var montopresupuestostr = montopresupuesto.Replace(".", "");
                decimal montofinal = decimal.Parse(montopresupuestostr);
                int idsucursalcliente = await GetIdClienteSucrusal(idcliente, idsucursal);
                int idcodigoccosto = await GetCostoUNegocioIdAsync(idcosto, idunegocio);
                List<ServicioViewModel> servicios = new List<ServicioViewModel>();
                var idsservicios = Request.Form["idservicio"];
                var idsegmentoservicio = Request.Form["idsegmentoservicio"];
                var montoservicioList = Request.Form["montoservicio"]; 

                for (int i = 0; i < idsservicios.Count; i++)
                {
                    var montoservicioStr = montoservicioList[i].ToString();

                    montoservicioStr = montoservicioStr.Replace(".", "");

                    decimal montoservicio = decimal.Parse(montoservicioStr);
                    var servicioViewModel = new ServicioViewModel
                    {
                        Idservicios = int.Parse(idsservicios[i]),
                        IdSegmento = int.Parse(idsegmentoservicio[i]),
                        MontoServicio = montoservicio, 
                    };

                    servicios.Add(servicioViewModel);
                }

                List<GastoViewModel> gastos = new List<GastoViewModel>();
                var idgastos = Request.Form["idgastos[]"]; 
                var idsegmentogasto = Request.Form["idsegmentogasto"];
                var montogastoList = Request.Form["montogasto"]; 

                for (int i = 0; i < idgastos.Count; i++)
                {
                    var gastoviewmodel = new GastoViewModel
                    {
                        Idgastos = int.Parse(idgastos[i]),
                        IdSegmento = int.Parse(idsegmentogasto[i]),
                        MontoGasto = decimal.Parse(montogastoList[i].ToString().Replace(".", "")) 
                    };

                    gastos.Add(gastoviewmodel);
                }

                bool resultado = await proyectoService.CrearProyecto(montofinal, moneda,afectaiva,idtipologia,nombre,numproyecto,fechainicio,fechatermino,plazo,tipoempresa, idcodigoccosto, idsucursalcliente,status,probabilidad,porcentajeprobabilidad,fechaplazoneg, hhsocios,hhstaff, hhconsultora, hhconsultorb,hhconsultorc,  idsegmentosocio,  idsegmentostaff,  idsegmentoconsultora,  idsegmentoconsultorb,  idsegmentoconsultorc,  idsegmentofactura, servicios, gastos);
                if (resultado)
                {
                    int idproyectoultimo = ultimoidproyecto();
                    return RedirectToAction("ObtenerProyectos","Proyecto",new {id= idproyectoultimo });
                }
                else
                {
                    ViewBag.ErrorMessage = "Hubo un error al registrar el proyecto.";
                    return View("NuevoProyecto");
                }
                
            }
            catch (Exception ex) {
                Log.Error(ex, "Error al crear el proyecto");
                ViewBag.ErrorMessage = $"Hubo un error al registrar el proyecto: {ex.Message}";
                Debug.WriteLine($"Hubo un error al registrar el proyecto:{ex.Message}");

                return View("NuevoProyecto");
            }
        }


       
        public async Task<IActionResult> ObtenerProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyectos = await proyectoService.ObtenerProyectos(id,idcliente,nombre,idtipoempresa,statusproyecto,numproyecto,idtipologia,unidadneg,idccosto, idusuario);
            var servicios = await proyectoService.ObtenerServiciosProyecto(id);
            var gastos = await proyectoService.ObtenerGastosProyectos(id);
            var facturas = await facturaService.RecuperarFacturas(id);
            
            ViewBag.Facturas = facturas;
            ViewBag.Proyectos = proyectos;
            ViewBag.Servicios = servicios;
            ViewBag.Gastos = gastos;
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
        public async Task<IActionResult> EditarProyecto(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto,int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            var unegocios = await proyectoService.ObtenerUnegocio();
            var ccostos = await proyectoService.ObtenerCcosto();
            var empresas = await proyectoService.ObtenerEmpresa();
            var tipologias = await proyectoService.ObtenerTipoligias();
            var clientes = await clienteService.ObtenerClientesIndex(0);
            var status = await RecuperarEstados();
            var recursos = await usuarioService.ObtenerUusario(0, null, 0);
            var segmentoscostos = await ObtenerSegmentosCostos();
            var sucursales = await ObtenerSucursales(idcliente);
            var serviciosproyectos = await proyectoService.ObtenerServiciosProyecto(id);
            var gastosproyectos = await proyectoService.ObtenerGastosProyectos(id);
            var servicios = await GetServicios();
            var gastos = await GetGastos();
            ViewBag.Gastos = gastos;
            ViewBag.GastosProyectos = gastosproyectos;
            ViewBag.Servicios = servicios;
            ViewBag.ServiciosProyectos = serviciosproyectos;
            ViewBag.SegmentoCosto = segmentoscostos;
            ViewBag.Recursos = recursos;
            ViewBag.Clientes = clientes;
            ViewBag.Tipologias = tipologias;
            ViewBag.Empresas = empresas;
            ViewBag.Ccostos = ccostos;
            ViewBag.Unegocios = unegocios;
            ViewBag.Status = status;
            ViewBag.Proyectos = proyecto;
            ViewBag.Sucursales = sucursales;
            return View();
        }

        public async Task<int> Obtenerstatusproyecto(int idproyecto)
        {
            var statusProyecto = await context.Proyectos
            .Where(p => p.Id == idproyecto)
            .Select(p => p.StatusProyecto)
            .FirstOrDefaultAsync();

            return statusProyecto;

        }

        [HttpPost]
        public async Task<IActionResult> ActualizarProyecto(int idproyecto, int idpresupuesto, decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura)
        {
            var statusproyecto = await Obtenerstatusproyecto(idproyecto);
            if (statusproyecto == 2)
            {
                List<ServicioViewModel> serviciosejecucion = new List<ServicioViewModel>();
                var idsserviciosejecucion = Request.Form["idservicio"];
                var idsegmentoservicioejecucion = Request.Form["idsegmentoservicio"];
                var montoservicioListejecucion = Request.Form["montoservicio"];

                for (int i = 0; i < idsserviciosejecucion.Count; i++)
                {
                    var montoservicioStr = montoservicioListejecucion[i].ToString();

                    montoservicioStr = montoservicioStr.Replace(".", "");

                    decimal montoservicio = decimal.Parse(montoservicioStr);
                    var servicioViewModel = new ServicioViewModel
                    {
                        Idservicios = int.Parse(idsserviciosejecucion[i]),
                        IdSegmento = int.Parse(idsegmentoservicioejecucion[i]),
                        MontoServicio = montoservicio,
                    };

                    serviciosejecucion.Add(servicioViewModel);
                }

                List<GastoViewModel> gastosejecucion = new List<GastoViewModel>();
                var idgastosejecuion = Request.Form["idgastos[]"];
                var idsegmentogastoejecucion = Request.Form["idsegmentogasto"];
                var montogastoListejecucion = Request.Form["montogasto"];

                for (int i = 0; i < idgastosejecuion.Count; i++)
                {
                    var montogastoStr = montogastoListejecucion[i].ToString();

                    montogastoStr = montogastoStr.Replace(".", "");

                    decimal montogasto = decimal.Parse(montogastoStr);
                    var gasto = new GastoViewModel
                    {
                        Idgastos = int.Parse(idgastosejecuion[i]),
                        IdSegmento = int.Parse(idsegmentogastoejecucion[i]),
                        MontoGasto = montogasto,
                    };

                    gastosejecucion.Add(gasto);
                }
                await proyectoService.GestorServiciosProyecto(idproyecto, serviciosejecucion);
                await proyectoService.GestorProyectoGastos(idproyecto, gastosejecucion);
                return RedirectToAction("ObtenerProyectos", "Proyecto", new { id = idproyecto });
            }

            int idcosto = int.Parse(Request.Form["centroCosto"]);
            int idunegocio = int.Parse(Request.Form["unidadNegocio"]);
            var montopresupuesto = Request.Form["monto"].ToString();
            var montopresupuestostr = montopresupuesto.Replace(".", "");
            decimal montofinal = decimal.Parse(montopresupuestostr); 
            int idcodigoccosto = await GetCostoUNegocioIdAsync(idcosto, idunegocio);
            List<ServicioViewModel> servicios = new List<ServicioViewModel>();
            var idsservicios = Request.Form["idservicio"];
            var idsegmentoservicio = Request.Form["idsegmentoservicio"];
            var montoservicioList = Request.Form["montoservicio"];

            for (int i = 0; i < idsservicios.Count; i++)
            {
                var montoservicioStr = montoservicioList[i].ToString();

                montoservicioStr = montoservicioStr.Replace(".", "");

                decimal montoservicio = decimal.Parse(montoservicioStr);
                var servicioViewModel = new ServicioViewModel
                {
                    Idservicios = int.Parse(idsservicios[i]),
                    IdSegmento = int.Parse(idsegmentoservicio[i]),
                    MontoServicio = montoservicio,
                };

                servicios.Add(servicioViewModel);
            }

            List<GastoViewModel> gastos = new List<GastoViewModel>();
            var idgastos = Request.Form["idgastos[]"];
            var idsegmentogasto = Request.Form["idsegmentogasto"];
            var montogastoList = Request.Form["montogasto"];
            
            for (int i = 0; i < idgastos.Count; i++)
            {
                var montogastoStr = montogastoList[i].ToString();

                montogastoStr = montogastoStr.Replace(".", "");

                decimal montogasto = decimal.Parse(montogastoStr);
                var gasto = new GastoViewModel
                {
                    Idgastos = int.Parse(idgastos[i]),
                    IdSegmento = int.Parse(idsegmentogasto[i]),
                    MontoGasto = montogasto,
                };

                gastos.Add(gasto);
            }
            List<UsuarioProyectoViewModel> usuariohoras = new List<UsuarioProyectoViewModel>();
            var statusedicion = int.Parse(Request.Form["status"].ToString());
            if (statusedicion == 2)
            {
                var idusuarios = Request.Form["idusuarios[]"];
                var hhasignadas = Request.Form["hhasignadas"];
                for (int i = 0; i < idusuarios.Count; i++)
                {
                    var usuarioh = new UsuarioProyectoViewModel
                    {
                        IdUsuario = int.Parse(idusuarios[i]),
                        HHAsignadas = int.Parse(hhasignadas[i]),
                    };
                    usuariohoras.Add(usuarioh);
                }
            }
            
            
            



            bool resultado = await proyectoService.EditarProyecto(idproyecto,idpresupuesto,montofinal,moneda,afectaiva,idtipologia,nombre,fechainicio,fechatermino,plazo,tipoempresa, idcodigoccosto,status,probabilidad,porcentajeprobabilidad,fechaplazoneg,hhsocios,hhstaff,hhconsultora,hhconsultorb,hhconsultorc,idsegmentosocio,idsegmentostaff,idsegmentoconsultora,idsegmentoconsultorb,idsegmentoconsultorc,idsegmentofactura, servicios, gastos, usuariohoras);
            
           
            if (resultado)
            {
                return RedirectToAction("ObtenerProyectos", "Proyecto", new {id=idproyecto});
            }
            else
            {
                return View();
            }

        }


        public async Task<List<StatusProyecto>> RecuperarEstados()
        {
            var estados = await context.StatusProyectos.ToListAsync();
            return estados;
        }
        public async Task<List<Servicio>> GetServicios()
        {
            var resultado = await context.Servicios
            .ToListAsync();
            return resultado;
        }

        public async Task<List<Gasto>> GetGastos()
        {
            var resultado = await context.Gastos
            .ToListAsync();
            return resultado;
        }

        public async Task<List<Sucursal>> ObtenerSucursales(int? idcliente)
        {
            var sucursales = await (from sc in context.SucursalClientes 
                                    join c in context.Sucursals on sc.IdSucursal equals c.Id 
                                    where sc.IdCliente == idcliente 
                                    select new
                                    {
                                        c.Id,
                                        c.Nombre
                                    }).ToListAsync();

            return sucursales.Select(s => new Sucursal
            {
                Id = s.Id,
                Nombre = s.Nombre
            }).ToList();
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
        public async Task<IActionResult> GetValoresGastosEdicion(int idcosto, int unegocio, string nombresegmento)
        {
            var gastos = await proyectoService.ObtenerValoresGastosEdicion(idcosto,unegocio, nombresegmento);
            return Json(gastos);
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
        public async Task<IActionResult> ObtenerValoresHonorarios(int idcosto, int idunegocio, int idrecurso)
        {
            var honorarios = await proyectoService.ObtenerValoresHonorarios(idcosto,idunegocio,idrecurso);
            return Json(honorarios);
        }
        

        public IActionResult GetProyectosUnidadNegocio()
        {
            return View("UnidadNegocio");
        }
    }
}
