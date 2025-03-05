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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;
using iText.Commons.Actions.Contexts;
namespace Proyectogestionhoras.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly ClienteService clienteService;
        private readonly UsuarioService usuarioService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly FacturaService facturaService;
        private readonly ReporteService reporteService;
        private readonly EjecucionService ejecucionService;
        public ProyectoController(ProyectoService proyectoService, ClienteService clienteService, UsuarioService usuarioService, PROYECTO_CONTROL_HORASContext context, FacturaService facturaService, ReporteService reporteService, EjecucionService ejecucionService)
        {
            this.proyectoService = proyectoService;
            this.clienteService = clienteService;
            this.usuarioService = usuarioService;
            this.context = context;
            this.facturaService = facturaService;
            this.reporteService = reporteService;
            this.ejecucionService = ejecucionService;
        }


        public async Task<IActionResult> GetProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {
                        var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
                        var proyectospresupuesto = await ObtenerPresupuestosProyecto();
                        ViewBag.Proyectos = proyectos;
                        ViewBag.Presupuesto = proyectospresupuesto;
                        return View("Proyectos");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");

        }

        public async Task<List<PresupuestoProyectoDTO>> ObtenerPresupuestosProyecto()
        {
            var proyectos = await context.Proyectos
                .Where(p => new[] { 1, 2, 6 }.Contains(p.StatusProyecto))
                .Select(p => new PresupuestoProyectoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    NumProyecto = p.NumProyecto,
                    NombreCliente = p.IdClienteSucursalNavigation.IdClienteNavigation.Nombre
                })
                .ToListAsync();

            return proyectos;
        }

        public async Task<IActionResult> NuevoProyecto()
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {
                        var totalhhanuales = await context.TotalRecursos.Where(t => t.Anio == DateTime.Now.Year).ToListAsync();
                        var unegocios = await proyectoService.ObtenerUnegocio();
                        var ccostos = await proyectoService.ObtenerCcosto();
                        var empresas = await proyectoService.ObtenerEmpresa();
                        var tipologias = await proyectoService.ObtenerTipoligias();
                        var costopromedio = await context.CostoPromedios.ToListAsync();
                        var status = await proyectoService.ObtenerStatus();
                        var recursos = await usuarioService.ObtenerUusario(0, null, 0);
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
                        ViewBag.Totales = totalhhanuales;
                        ViewBag.Costos = costopromedio;
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");


        }


        [HttpPost]
        public async Task<IActionResult> CrearProyecto(decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, string numproyecto, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int idclientesucursal, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura, decimal montoorigen, decimal tasacambio, int cuotas)
        {
            try
            {

                int idcosto = int.Parse(Request.Form["centroCosto"]);
                int idunegocio = int.Parse(Request.Form["unidadNegocio"]);

                int idcliente = int.Parse(Request.Form["cliente"]);
                int idsucursal = int.Parse(Request.Form["sucursal"]);
                var montopresupuesto = Request.Form["monto"].ToString();
                var montopresupuestostr = montopresupuesto.Replace(".", "");
                decimal montofinal = decimal.Parse(montopresupuestostr, System.Globalization.CultureInfo.InvariantCulture);
                int idsucursalcliente = await GetIdClienteSucrusal(idcliente, idsucursal);
                int idcodigoccosto = await GetCostoUNegocioIdAsync(idcosto, idunegocio);
                List<ServicioViewModel> servicios = new List<ServicioViewModel>();
                var idsservicios = Request.Form["idservicio"];
                var idsegmentoservicio = Request.Form["idsegmentoservicio"];
                var montoservicioList = Request.Form["montoservicio"];
                var fechaservicio = Request.Form["fechaservicio"];
                var IdServicioProyecto = Request.Form["IdServicioProyecto"];
                for (int i = 0; i < idsservicios.Count; i++)
                {
                    int idServicioRealParsed = string.IsNullOrWhiteSpace(IdServicioProyecto[i])
                                           ? 0
                                           : int.Parse(IdServicioProyecto[i]);
                    var montoservicioStr = montoservicioList[i].ToString();

                    montoservicioStr = montoservicioStr.Replace(".", "");

                    decimal montoservicio = decimal.Parse(montoservicioStr);
                    var servicioViewModel = new ServicioViewModel
                    {
                        IdServicioProyecto = idServicioRealParsed,
                        Idservicios = int.Parse(idsservicios[i]),
                        IdSegmento = int.Parse(idsegmentoservicio[i]),
                        MontoServicio = montoservicio,
                        Fecha = DateTime.Parse(fechaservicio[i]),
                        espresupuesto = "Si",
                    };

                    servicios.Add(servicioViewModel);
                }

                List<GastoViewModel> gastos = new List<GastoViewModel>();
                var idgastos = Request.Form["idgastos[]"];
                var idsegmentogasto = Request.Form["idsegmentogasto"];
                var montogastoList = Request.Form["montogasto"];
                var fechagasto = Request.Form["fechagasto"];
                var idgastoproyecto = Request.Form["IdGastoProyecto"];
                for (int i = 0; i < idgastos.Count; i++)
                {
                    int idGastoRealParsed = string.IsNullOrWhiteSpace(idgastoproyecto[i])
                                           ? 0
                                           : int.Parse(idgastoproyecto[i]);
                    var gastoviewmodel = new GastoViewModel
                    {
                        IdGastoProyecto = idGastoRealParsed,
                        Idgastos = int.Parse(idgastos[i]),
                        IdSegmento = int.Parse(idsegmentogasto[i]),
                        MontoGasto = decimal.Parse(montogastoList[i].ToString().Replace(".", "")),
                        Fecha = DateTime.Parse(fechagasto[i]),
                        espresupuesto = "Si",
                    };

                    gastos.Add(gastoviewmodel);
                }
                if (Request.Form["hhsocios"].ToString() == "")
                {
                    hhsocios = 0;
                }
                if (Request.Form["hhstaff"].ToString() == "")
                {
                    hhstaff = 0;
                }
                if (Request.Form["hhconsultora"].ToString() == "")
                {
                    hhconsultora = 0;
                }
                if (Request.Form["hhconsultorb"].ToString() == "")
                {
                    hhconsultorb = 0;
                }
                if (Request.Form["hhconsultorc"].ToString() == "")
                {
                    hhconsultorc = 0;
                }

                decimal montoorigenextranjera = 0;
                decimal tasacambios = 0;
                var montomonedaorigentr = Request.Form["montoorigen"].ToString().Replace(".", "");
                var tasacambiotr = Request.Form["tasacambio"].ToString().Replace(".", "");
                montoorigenextranjera = decimal.Parse(montomonedaorigentr, System.Globalization.CultureInfo.InvariantCulture);
                tasacambios = decimal.Parse(tasacambiotr, System.Globalization.CultureInfo.InvariantCulture);


                /*ingresando cuotas*/
                List<CuotasViewModel> cuota = new List<CuotasViewModel>();

                var numdocumento = Request.Form["numdocumento"];
                var fechaemision = Request.Form["fechaemision"];

                var Montoclplist = Request.Form["montocuota"];

                var Observacion = Request.Form["observacioncuota"];
                var idingresoreal = Request.Form["IdIngresoreal"];


                for (int i = 0; i < numdocumento.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(numdocumento[i]))
                    {
                        continue;
                    }

                    string montosclpStr = Montoclplist[i]?.ToString().Trim() ?? "0";


                    decimal.TryParse(montosclpStr.Replace(".", ""), out decimal montoclp);

                    int.TryParse(idingresoreal[i]?.ToString(), out int idIngresoRealParsed);



                    DateTime fechaemisionParsed = DateTime.TryParse(fechaemision[i], out DateTime tempDate)
                        ? tempDate
                        : DateTime.Today;


                    var cuotaViewModel = new CuotasViewModel
                    {
                        IdCuota = idIngresoRealParsed,
                        FechaEmision = fechaemisionParsed,
                        MontoCuota = montoclp,
                        Observacion = Observacion[i],

                    };
                    cuota.Add(cuotaViewModel);

                }

                bool resultado = await proyectoService.CrearProyecto(montofinal, moneda, afectaiva, idtipologia, nombre, numproyecto, fechainicio, fechatermino, plazo, tipoempresa, idcodigoccosto, idsucursalcliente, probabilidad, porcentajeprobabilidad, fechaplazoneg, hhsocios, hhstaff, hhconsultora, hhconsultorb, hhconsultorc, idsegmentosocio, idsegmentostaff, idsegmentoconsultora, idsegmentoconsultorb, idsegmentoconsultorc, idsegmentofactura, montoorigenextranjera, tasacambios, cuotas, servicios, gastos, cuota);
                if (resultado)
                {
                    int idproyectoultimo = ultimoidproyecto();
                    await proyectoService.GestorFechaModificacionProyecto(idproyectoultimo);
                    return RedirectToAction("ObtenerPresupuestoProyecto", "Proyecto", new { id = idproyectoultimo });
                }
                else
                {
                    ViewBag.ErrorMessage = "Hubo un error al registrar el proyecto.";
                    return View("NuevoProyecto");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al crear el proyecto");
                ViewBag.ErrorMessage = $"Hubo un error al registrar el proyecto: {ex.Message}";
                Debug.WriteLine($"Hubo un error al registrar el proyecto:{ex.Message}");

                return View("NuevoProyecto");
            }
        }

        public async Task<int> Obtenerstatusproyectos(int? idproyecto)
        {
            var statusProyecto = await context.Proyectos
            .Where(p => p.Id == idproyecto)
            .Select(p => p.StatusProyecto)
            .FirstOrDefaultAsync();

            return statusProyecto;

        }

        public async Task<IActionResult> ObtenerProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {
                        int status = await Obtenerstatusproyectos(id);
                        if (status == 6)
                        {
                            return RedirectToAction("ObtenerPresupuestoProyecto", "Proyecto", new { id = id });
                        }
                        var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
                        var servicios = await proyectoService.ObtenerServiciosProyecto(id);
                        var gastos = await proyectoService.ObtenerGastosProyectos(id);
                        var facturas = await facturaService.RecuperarFacturas(id);
                        var forecastreal = await reporteService.ObtenerForecastrealProyecto(id);
                        var fechaactualizacionproyecto = await context.Fechamodificacionproyectos.Where(f => f.Idproyecto == id).FirstOrDefaultAsync();
                        ViewBag.Facturas = facturas;
                        ViewBag.Proyectos = proyectos;
                        ViewBag.Servicios = servicios;
                        ViewBag.Gastos = gastos;
                        ViewBag.ForecastReal = forecastreal;
                        ViewBag.FechaModificacion = fechaactualizacionproyecto?.Fechamodificacion?.ToString("dd/MM/yyyy 'a las' HH:mm") ?? "Sin actualización";
                        return View("DetalleProyecto");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");


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



        public async Task<IActionResult> EditarProyecto(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1)
                    {
                        var cuotasproyecto = await context.Cuotas.Where(x => x.Idpresupuesto == id).ToListAsync();
                        var totalhhanuales = await context.TotalRecursos.Where(t => t.Anio == DateTime.Now.Year).ToListAsync();
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
                        var historialcosto = await context.HistorialCostosProyectos.Where(p => p.Idproyecto == id).ToListAsync();
                        var costopromedio = await context.CostoPromedios.ToListAsync();
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
                        ViewBag.Totales = totalhhanuales;
                        ViewBag.Costos = costopromedio;
                        ViewBag.HistorialCosto = historialcosto;
                        ViewBag.Cuotas = cuotasproyecto;
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");

        }

        public async Task<int> Obtenerstatusproyecto(int idproyecto)
        {
            var statusProyecto = await context.Proyectos
            .Where(p => p.Id == idproyecto)
            .Select(p => p.StatusProyecto)
            .FirstOrDefaultAsync();

            return statusProyecto;

        }


        public async Task ActualizarFechaQuarterNegociacion(int idproyecto,DateTime? fechainicioquarter,DateTime? fechafinquarter)
        {
            var proyecto = context.Proyectos.Find(idproyecto);
            if (proyecto != null) { 
            
                proyecto.Fechaquarterinicio = fechainicioquarter;
                proyecto.Fechaquarterfin = fechafinquarter;
                await context.SaveChangesAsync();
            }
            else
            {
                return ;
            }
        }

        public async Task ActualizarProyectoEjecucion(int idproyecto, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc,DateTime? fechaquarterinicio, DateTime? fechaquarterfin)
        {

            var presupuesto = context.Presupuestos.FirstOrDefault(p => p.Id == idproyecto);
            if (presupuesto != null)
            {

                presupuesto.Cantidadcuotas = int.Parse(Request.Form["cuotas"].ToString());


                context.SaveChanges();
            }


            List<ServicioViewModel> serviciosejecucion = new List<ServicioViewModel>();
            var idsserviciosejecucion = Request.Form["idservicio"];
            var idsegmentoservicioejecucion = Request.Form["idsegmentoservicio"];
            var montoservicioListejecucion = Request.Form["montoservicio"];
            var fechaservicioejecucion = Request.Form["fechaservicio"];
            var esEliminado = Request.Form["esEliminado"];
            var IdServicioProyecto = Request.Form["IdServicioProyecto"];


            for (int i = 0; i < idsserviciosejecucion.Count; i++)
            {
                int idServicioRealParsed = string.IsNullOrWhiteSpace(IdServicioProyecto[i])
                                       ? 0
                                       : int.Parse(IdServicioProyecto[i]);
                var servicioViewModel = new ServicioViewModel
                {
                    IdServicioProyecto = idServicioRealParsed,
                    Idservicios = int.Parse(idsserviciosejecucion[i]),
                    IdSegmento = int.Parse(idsegmentoservicioejecucion[i]),
                    MontoServicio = decimal.Parse(montoservicioListejecucion[i].Replace(".", "")),
                    Fecha = DateTime.Parse(fechaservicioejecucion[i]),
                    EsEliminado = esEliminado[i] == "true",

                };

                serviciosejecucion.Add(servicioViewModel);
            }



            List<GastoViewModel> gastosejecucion = new List<GastoViewModel>();
            var idgastosejecuion = Request.Form["idgastos[]"];
            var idsegmentogastoejecucion = Request.Form["idsegmentogasto"];
            var montogastoListejecucion = Request.Form["montogasto"];
            var fechagastoejecucion = Request.Form["fechagasto"];
            var esEliminadogasto = Request.Form["esEliminados"];
            var idgastoproyecto = Request.Form["IdGastoProyecto"];
            for (int i = 0; i < idgastosejecuion.Count; i++)
            {
                int idGastoRealParsed = string.IsNullOrWhiteSpace(idgastoproyecto[i])
                                       ? 0
                                       : int.Parse(idgastoproyecto[i]);
                var montogastoStr = montogastoListejecucion[i].ToString();

                montogastoStr = montogastoStr.Replace(".", "");

                decimal montogasto = decimal.Parse(montogastoStr);
                var gasto = new GastoViewModel
                {
                    IdGastoProyecto = idGastoRealParsed,
                    Idgastos = int.Parse(idgastosejecuion[i]),
                    IdSegmento = int.Parse(idsegmentogastoejecucion[i]),
                    MontoGasto = montogasto,
                    Fecha = DateTime.Parse(fechagastoejecucion[i]),
                    EsEliminado = esEliminadogasto[i] == "true",

                };

                gastosejecucion.Add(gasto);
            }

            /*Actualizar Cuotas*/
            int cantidadcuotas = int.Parse(Request.Form["cuotas"].ToString());
            await ActualizarCuotas(idproyecto, cantidadcuotas);
            await ActualizarFechaQuarterNegociacion(idproyecto,fechaquarterinicio,fechaquarterfin);
            await proyectoService.GestorServiciosProyecto(idproyecto, serviciosejecucion);
            await proyectoService.GestorProyectoGastos(idproyecto, gastosejecucion);
            await proyectoService.RestarHHAnaulesSocios(hhsocios, idproyecto);
            await proyectoService.RestarHHAnaulesStaff(hhstaff, idproyecto);
            await proyectoService.ReasignarHHRecursos(idproyecto, hhsocios, hhstaff, hhconsultora, hhconsultorb, hhconsultorc, idsegmentosocio, idsegmentostaff, idsegmentoconsultora, idsegmentoconsultorb, idsegmentoconsultorc);
            await proyectoService.GestorFechaModificacionProyecto(idproyecto);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarProyecto(int idproyecto, int idpresupuesto, decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura, int cuotas,DateTime? fechaquarterinicio,DateTime? fechaquarterfin)
        {

            var idclientes = Request.Form["idcliente"];
            try
            {
                /*EJECUCION*/
                var statusproyecto = await Obtenerstatusproyecto(idproyecto);
                if (statusproyecto == 2)
                {
                    await ActualizarProyectoEjecucion(idproyecto, hhsocios, hhstaff, hhconsultora, hhconsultorb, hhconsultorc, idsegmentosocio, idsegmentostaff, idsegmentoconsultora, idsegmentoconsultorb, idsegmentoconsultorc,fechaquarterinicio,fechaquarterfin);
                 
                    return RedirectToAction("ObtenerProyectos", "Proyecto", new { id = idproyecto });
                }


                /*OTROS ESTADOS*/
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
                var fechaservicio = Request.Form["fechaservicio"];
                var esEliminados = Request.Form["esEliminado"];
                var IdServicioProy = Request.Form["IdServicioProyecto"];

                var statusedicion = int.Parse(Request.Form["status"].ToString());


                for (int i = 0; i < idsservicios.Count; i++)
                {
                    int idServicioRealParsed = string.IsNullOrWhiteSpace(IdServicioProy[i])
                                      ? 0
                                      : int.Parse(IdServicioProy[i]);
                    var montoservicioStr = montoservicioList[i].ToString();

                    montoservicioStr = montoservicioStr.Replace(".", "");

                    decimal montoservicio = decimal.Parse(montoservicioStr);
                    var servicioViewModel = new ServicioViewModel
                    {
                        IdServicioProyecto = idServicioRealParsed,
                        Idservicios = int.Parse(idsservicios[i]),
                        IdSegmento = int.Parse(idsegmentoservicio[i]),
                        MontoServicio = montoservicio,
                        Fecha = DateTime.Parse(fechaservicio[i]),
                        EsEliminado = esEliminados[i] == "true",

                    };

                    servicios.Add(servicioViewModel);
                }

                List<GastoViewModel> gastos = new List<GastoViewModel>();
                var idgastos = Request.Form["idgastos[]"];
                var idsegmentogasto = Request.Form["idsegmentogasto"];
                var montogastoList = Request.Form["montogasto"];
                var fechagasto = Request.Form["fechagasto"];
                var esEliminadogastos = Request.Form["esEliminados"];
                var idgastoproyectos = Request.Form["IdGastoProyecto"];
                for (int i = 0; i < idgastos.Count; i++)
                {
                    int idGastoRealParsed = string.IsNullOrWhiteSpace(idgastoproyectos[i])
                                           ? 0
                                           : int.Parse(idgastoproyectos[i]);
                    var montogastoStr = montogastoList[i].ToString();

                    montogastoStr = montogastoStr.Replace(".", "");

                    decimal montogasto = decimal.Parse(montogastoStr);
                    var gasto = new GastoViewModel
                    {
                        IdGastoProyecto = idGastoRealParsed,
                        Idgastos = int.Parse(idgastos[i]),
                        IdSegmento = int.Parse(idsegmentogasto[i]),
                        MontoGasto = montogasto,
                        Fecha = DateTime.Parse(fechagasto[i]),
                        EsEliminado = esEliminadogastos[i] == "true",

                    };

                    gastos.Add(gasto);
                }

                decimal montoorigenextranjera = 0;
                decimal tasacambios = 0;
                var montomonedaorigentr = Request.Form["montoorigen"].ToString().Replace(".", "");
                var tasacambiotr = Request.Form["tasacambio"].ToString().Replace(".", "");
                montoorigenextranjera = decimal.Parse(montomonedaorigentr, System.Globalization.CultureInfo.InvariantCulture);
                tasacambios = decimal.Parse(tasacambiotr, System.Globalization.CultureInfo.InvariantCulture);


                if (statusedicion == 2)
                {

                    await proyectoService.RestarHHAnualesCambioEstadoSocios(hhsocios);
                    await proyectoService.RestarHHAnualesCambioEstadoStaff(hhstaff);

                }




                int cantidadcuotas = int.Parse(Request.Form["cuotas"].ToString());

                bool resultado = await proyectoService.EditarProyecto(idproyecto, idpresupuesto, montofinal, moneda, afectaiva, idtipologia, nombre, fechainicio, fechatermino, plazo, tipoempresa, idcodigoccosto, status, probabilidad, porcentajeprobabilidad, fechaplazoneg, hhsocios, hhstaff, hhconsultora, hhconsultorb, hhconsultorc, idsegmentosocio, idsegmentostaff, idsegmentoconsultora, idsegmentoconsultorb, idsegmentoconsultorc, idsegmentofactura, montoorigenextranjera, tasacambios, cantidadcuotas, fechaquarterinicio,fechaquarterfin,servicios, gastos);
                /*Actualizar Cuotas*/

                await ActualizarCuotas(idproyecto, cantidadcuotas);
                await proyectoService.GestorFechaModificacionProyecto(idproyecto);

                if (resultado)
                {
                    if (statusedicion == 6)
                    {
                        return RedirectToAction("ObtenerPresupuestoProyecto", "Proyecto", new { id = idproyecto });
                    }
                    else
                    {
                        return RedirectToAction("ObtenerProyectos", "Proyecto", new { id = idproyecto });
                    }

                }
                else
                {

                    return View("EditarProyecto", new { id = idproyecto, idcliente = idclientes });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al editar el proyecto:{e.Message}");
                TempData["ErrorMessage"] = "Hubo un error al Editar el proyecto.";
                return RedirectToAction("EditarProyecto", new { id = idproyecto, idcliente = idclientes });
            }


        }

        public async Task ActualizarCuotas(int idproyecto, int cantidadcuotas)
        {
            List<CuotasViewModel> cuota = new List<CuotasViewModel>();

            /*eliminando cuotas*/
            int cantidadcuotass = await context.Cuotas.Where(c => c.Idpresupuesto == idproyecto).CountAsync();
            if (cantidadcuotas < cantidadcuotass)
            {
                int cantidadcuotasaeliminar = int.Parse(Request.Form["filasEliminadas"].ToString());
                var registrosAEliminar = await context.Cuotas
                .Where(c => c.Idpresupuesto == idproyecto)
                .OrderByDescending(c => c.Id)
                .Take(cantidadcuotasaeliminar)
                .ToListAsync();

                context.Cuotas.RemoveRange(registrosAEliminar);
                context.SaveChanges();
            }

            /*ingresando cuotas*/
            var fechaemision = Request.Form["fechaemision"];

            var Montoclplist = Request.Form["montocuota"];

            var Observacion = Request.Form["observacioncuota"];
            var idingresoreal = Request.Form["IdCuota"];


            for (int i = 0; i < fechaemision.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(fechaemision[i]))
                {
                    continue;
                }


                string montosclpStr = Montoclplist[i]?.ToString().Trim() ?? "0";


                decimal.TryParse(montosclpStr.Replace(".", ""), out decimal montoclp);

                int.TryParse(idingresoreal[i]?.ToString(), out int idIngresoRealParsed);



                DateTime fechaemisionParsed = DateTime.TryParse(fechaemision[i], out DateTime tempDate)
                    ? tempDate
                    : DateTime.Today;


                var CuotaViewModel = new CuotasViewModel
                {
                    IdCuota = idIngresoRealParsed,

                    FechaEmision = fechaemisionParsed,

                    MontoCuota = montoclp,

                    Observacion = Observacion[i],

                };

                cuota.Add(CuotaViewModel);

            }

            await proyectoService.GestorCuotas(idproyecto, cuota);
        }


        [HttpPost]
        public async Task<IActionResult> TerminarProyecto(int id)
        {

            var proyecto = await context.Proyectos.FindAsync(id);


            if (proyecto == null)
            {
                return NotFound();
            }


            proyecto.StatusProyecto = 4;
            proyecto.Fecharealtermino = DateTime.Now;

            await context.SaveChangesAsync();


            return RedirectToAction("ObtenerProyectos", "Proyecto", new { id = proyecto.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RetornarEjecucion(int id)
        {
            var proyecto = await context.Proyectos.FindAsync(id);
            if(proyecto == null)
            {
                return NotFound();
            }
            proyecto.StatusProyecto = 2;
            await context.SaveChangesAsync();
            return RedirectToAction("ObtenerProyectos", "Proyecto", new { id = proyecto.Id });
        }


        public async Task<IActionResult> ObtenerPresupuestoProyecto(int? idpresupuesto, int? id, int? idcliente)
        {
            var presupuesto = await proyectoService.ObtenerPresupuestoProyectos(idpresupuesto, id, idcliente);
            var facturas = await facturaService.RecuperarFacturas(id);
            var servicios = await proyectoService.ObtenerServiciosProyecto(id);
            var gastos = await proyectoService.ObtenerGastosProyectos(id);
            ViewBag.Gastos = gastos;
            ViewBag.Servicios = servicios;
            ViewBag.Presupuesto = presupuesto;
            ViewBag.Facturas = facturas;
            return View("PrespuestoProyecto");
        }



        public async Task<IActionResult> ObtenerhistorialNegociaciones(int? idproyecto, int? idnegociacion)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {
                        var historial = await proyectoService.RecuperarHistorialNegociacion(idproyecto, idnegociacion);
                        ViewBag.Historial = historial;
                        return View("Historial");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");

        }

        public async Task<IActionResult> DetalleNegociacion(int? idproyecto, int? idnegociación)
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1)
                    {
                        var historial = await proyectoService.RecuperarHistorialNegociacion(idproyecto, idnegociación);
                        var factura = await proyectoService.RecuperarFacturaNegociacion(idnegociación);
                        ViewBag.Historial = historial;
                        ViewBag.Facturas = factura;
                        return View("DetalleNegociacion");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");

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

                .Select(s => new Segmento { Id = s.Id, Nombre = s.Nombre, TipoSegmento = s.TipoSegmento })
                .ToListAsync();

            return segmentosCostos;
        }



        [HttpGet]
        public async Task<IActionResult> GetValoresGastosEdicion(int idcosto, int unegocio, int idgasto)
        {
            var gastos = await proyectoService.ObtenerValoresGastosEdicion(idcosto, unegocio, idgasto);
            return Json(gastos);
        }

        [HttpGet]
        public async Task<IActionResult> GetValoresServiciosEdicion(int idcosto, int unegocio, int idservicio)
        {
            var gastos = await proyectoService.ObtenerValoresServiciosEdicion(idcosto, unegocio, idservicio);
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
                if (anioultimoproyecto == anioactual)
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
        public async Task<IActionResult> Getcodigoccosto(int idcosto, int idunegocio)
        {
            var codigo = await proyectoService.ObtenerCodigoCCosto(idcosto, idunegocio);
            return Json(codigo);
        }

        [HttpGet]
        public async Task<IActionResult> GetValoresFactura(int idcosto, int idunegocio)
        {
            var factura = await proyectoService.ObtenerValoresFactura(idcosto, idunegocio);
            return Ok(factura);
        }
        [HttpGet]
        public async Task<IActionResult> GetValoresServicios(int idcosto, int idunegocio)
        {
            var servicio = await proyectoService.ObtenerValoresServicios(idcosto, idunegocio);
            return Ok(servicio);
        }

        [HttpGet]
        public async Task<IActionResult> GetSegmentosGastos(int idcodigo)
        {
            var segmentos = await proyectoService.GetValoresSegmentos(idcodigo);
            return Ok(segmentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerValoresGastos(int idcodigo, string nombresegmento)
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
            var consultores = await proyectoService.ObtenerValoresConsultores(idcosto, idunegocio);
            return Json(consultores);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerValoresHonorarios(int idcosto, int idunegocio, int idrecurso)
        {
            var honorarios = await proyectoService.ObtenerValoresHonorarios(idcosto, idunegocio, idrecurso);
            return Json(honorarios);
        }


        public async Task<IActionResult> GetProyectosCategoria()
        {
            var iduser = HttpContext.Session.GetInt32("id");
            if (iduser.HasValue)
            {
                var rol = HttpContext.Session.GetInt32("idrol");
                if (rol.HasValue)
                {
                    if (rol == 1 || rol == 3)
                    {

                        var tipologias = await proyectoService.ObtenerTipoligias();
                        var ccosto = await proyectoService.ObtenerCcosto();
                        var unegocio = await proyectoService.ObtenerUnegocio();

                        var ccostoUnegocios = await context.CcostoUnegocios
       .Include(c => c.IdCcostoNavigation)
       .Include(c => c.IdUnegocioNavigation)
       .ToListAsync();
                        var viewModel = new ProyectosCategoriaViewModel
                        {
                            Tipologias = tipologias,
                            Ccosto = ccosto,
                            Unegocio = unegocio,
                            CcostoUnegocios = ccostoUnegocios
                        };
                        return View("ProyectosCategoria", viewModel);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }


            return RedirectToAction("Login", "Usuario");

        }

        [HttpPost]
        public IActionResult FiltrarProyectos(int? tipologiaId, int? ccostoUnegocioId, int? unegocioId)
        {
            var proyectosFiltrados = context.Proyectos
                                            .Include(p => p.IdCcostoUnegocioNavigation)
                                            .ThenInclude(cu => cu.IdCcostoNavigation)
                                            .Include(p => p.IdCcostoUnegocioNavigation)
                                            .ThenInclude(cu => cu.IdUnegocioNavigation)
                                            .AsQueryable();

            if (tipologiaId.HasValue)
            {
                proyectosFiltrados = proyectosFiltrados.Where(p => p.IdTipologia == tipologiaId.Value);
            }

            if (ccostoUnegocioId.HasValue && ccostoUnegocioId.Value != 0)
            {
                proyectosFiltrados = proyectosFiltrados.Where(p => p.IdCcostoUnegocio == ccostoUnegocioId.Value);
            }
            else if (unegocioId.HasValue && unegocioId.Value != 0)
            {
                proyectosFiltrados = proyectosFiltrados
                    .Where(p => p.IdCcostoUnegocioNavigation.IdUnegocio == unegocioId.Value);
            }
            var proyectosList = proyectosFiltrados
                .Select(p => new
                {
                    p.Id,
                    p.NumProyecto,
                    p.Nombre,
                    NombreCliente = p.IdClienteSucursalNavigation.IdClienteNavigation.Nombre
                })
                .ToList();

            return Json(proyectosList);
        }




    }
}
