using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using System.Diagnostics;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyectogestionhoras.Controllers
{
    public class EjecucionProyectoController : Controller
    {
        private readonly ProyectoService proyectoService;
        private readonly FacturaService facturaService;
        private readonly EjecucionService ejecucionService;
        private readonly ReporteService reporteService;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public EjecucionProyectoController(ProyectoService proyectoService, FacturaService facturaService, EjecucionService ejecucionService, PROYECTO_CONTROL_HORASContext context, ReporteService reporteService)
        {
            this.proyectoService = proyectoService;
            this.facturaService = facturaService;
            this.ejecucionService = ejecucionService;
            this.context = context;
            this.reporteService = reporteService;
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

        /*FORECAST INGRESOS*/
        public async Task<IActionResult> ForecastIngreso(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            var ingresos = await context.Ingresosreales.Where(x => x.Idproyecto == id).ToListAsync();
            var flujocaja = await reporteService.ProcesarFlujoCajaPorMesAsync(id);
            ViewBag.Ingresos = ingresos;
            ViewBag.Proyecto = proyecto;
            return View(flujocaja);
        }



        [HttpPost]
        public async Task<IActionResult> RegistrarIngresos(int idproyecto)
        {
            List<IngresoViewModel> ingresos = new List<IngresoViewModel>();

            var numdocumento = Request.Form["Numdocumento"];
            var fechapago = Request.Form["FechaPago"];
            var fechaemision = Request.Form["FechaEmision"];
           // var montousdlist = Request.Form["Montous"];
           // var Tclist = Request.Form["Tc"];
            var Montoclplist = Request.Form["Montoclp"];
            var Ivalist = Request.Form["Iva"];
            var Estado = Request.Form["Estado"];
            var Idcuenta = Request.Form["Idcuenta"];
            var Observacion = Request.Form["Observacion"];
            var idingresoreal = Request.Form["IdIngresoreal"];

            for (int i = 0; i < numdocumento.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(numdocumento[i]))
                {
                    continue; 
                }
              
               // string montosusdStr = montousdlist[i]?.ToString().Trim() ?? "0";
                string montosclpStr = Montoclplist[i]?.ToString().Trim() ?? "0";
                string montosivaStr = Ivalist[i]?.ToString().Trim() ?? "0";
               // string tcStr = Tclist[i]?.ToString().Trim() ?? "0";

                
               // decimal.TryParse(montosusdStr.Replace(".", ""), out decimal montousd);
                decimal.TryParse(montosclpStr.Replace(".", ""), out decimal montoclp);
                decimal.TryParse(montosivaStr.Replace(".", ""), out decimal montoiva);
              //  decimal.TryParse(tcStr.Replace(".", ""), out decimal tc);

              
                int.TryParse(idingresoreal[i]?.ToString(), out int idIngresoRealParsed);
                int.TryParse(Idcuenta[i]?.ToString(), out int idCuentaParsed);

                
                DateTime fechaemisionParsed = DateTime.TryParse(fechaemision[i], out DateTime tempDate)
                    ? tempDate
                    : DateTime.Today;
                DateTime fechapagoparsed = DateTime.Parse(fechapago[i]);
               
                var ingresoViewModel = new IngresoViewModel
                {
                    IdIngresoreal = idIngresoRealParsed,
                    Numdocumento = numdocumento[i],
                    FechaEmision = fechaemisionParsed,
                    FechaPago = fechapagoparsed,
                   // Montous = montousd,
                   // Tc = tc,
                    Montoclp = montoclp,
                    Iva = montoiva,
                    Estado = Estado[i],
                    Idcuenta = idCuentaParsed,
                    Observacion = Observacion[i],
                };

                ingresos.Add(ingresoViewModel);
            }

            await ejecucionService.GestorIngresos(idproyecto, ingresos);
            return RedirectToAction("ForecastIngreso", "EjecucionProyecto", new { id = idproyecto });
        }

        [HttpGet]
        public async Task<IActionResult> RecuperarForecastIngresos()
        {
            var resultado = await ejecucionService.ObtenerForecastIngresos();
            return Json(resultado);
        }

        /*FORECAST COSTOS*/
       
        public async Task<IActionResult> ForecastCostos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            var serviciosejecucion = await context.Serviciosejecucions.Where(s => s.Idproyecto == id).ToListAsync();
            var gastosejecucion = await ejecucionService.ObtenerGastosReales(id);
            var servicios = await GetServicios();
            var gastos = await GetGastos();
            var proveedoresservicios = await GetProveedoresServicios();
            var proveedoresgastos = await GetProveedoresGastos();
            var gastoshh = await ejecucionService.ObtenerGastosHH(id);
            var datosgastosrecursos = await context.Gastoshhhejecucions.Where(g => g.Idproyecto == id).ToListAsync();
            var serviciosproyectados = await proyectoService.ObtenerServiciosProyecto(id);
            var gastosproyectados = await proyectoService.ObtenerGastosProyectos(id);
            ViewBag.Proyecto = proyecto;
            ViewBag.ServiciosEjecucion = serviciosejecucion;
            ViewBag.GastosEjecucion = gastosejecucion;
            ViewBag.Servicios = servicios;
            ViewBag.Gastos = gastos;
            ViewBag.Proveedores = proveedoresservicios;
            ViewBag.ProGastos = proveedoresgastos;
            ViewBag.GastosHH = gastoshh;
            ViewBag.GastosRecursos = datosgastosrecursos;
            ViewBag.ServiciosProyectos = serviciosproyectados;
            ViewBag.GastosProyectos = gastosproyectados;

            var serviciosreales = from serejecucucion in serviciosejecucion
                                  group serejecucucion by new { serejecucucion.Idservicio, serejecucucion.Estado } into grupo
                                  select new
                                  {
                                      Idservicio = grupo.Key.Idservicio,
                                      Estado = grupo.Key.Estado,
                                      TotalMonto = grupo.Sum(x => x.Monto)
                                  };

            
            ViewBag.ServiciosTotalesPagados = serviciosreales
                .Where(x => x.Estado == "Pagada")
                .ToDictionary(x => x.Idservicio, x => x.TotalMonto);

            ViewBag.ServiciosTotalesForecast = serviciosreales
                .Where(x => x.Estado == "Forecast")
                .ToDictionary(x => x.Idservicio, x => x.TotalMonto);

            var gastosTotales = from gasejecucion in gastosejecucion
                                group gasejecucion by new { gasejecucion.IdGasto,gasejecucion.Estado} into grupo
                                select new
                                {
                                    IdGasto = grupo.Key.IdGasto,
                                    Estado = grupo.Key.Estado,
                                    TotalMonto = grupo.Sum(x => x.Monto)
                                };

            ViewBag.GastosTotalesPagados = gastosTotales
                .Where(x => x.Estado == "Pagada")
                .ToDictionary(x => x.IdGasto, x => x.TotalMonto);

            ViewBag.GastosTotalesForecast = gastosTotales
                .Where(x => x.Estado == "Forecast")
                .ToDictionary(x => x.IdGasto, x => x.TotalMonto);

            return View();
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

        public async Task<List<Proveedore>> GetProveedoresServicios()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Servicio%")).OrderBy(p => p.Nombre)
            .ToListAsync();
            return proveedores;
        }
        public async Task<List<Proveedore>> GetProveedoresGastos()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Gasto%")).OrderBy(p => p.Nombre)
            .ToListAsync();
            return proveedores;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCostos(int idproyecto, List<GastosHHViewModel> gastosHH)
        {
            try
            {
                /*COSTOS SERVICIOS REALES*/

                var idsservicios = Request.Form["Idservicio"];
                var idproveedores = Request.Form["Idproveedor"];
                var montoservicioList = Request.Form["montoservicio"];
                var fechaservicio = Request.Form["fechaservicio"];
                var idservicioreal = Request.Form["IdServicioReal"];
                var observacion = Request.Form["observacionservicio"];
                var estadoservicio = Request.Form["Estado"];
                var tiposervicio = Request.Form["Tiposervicio"];
                var idsegmentootro = Request.Form["idsegmentoserviciootro"];
                var eliminarServicioOtro = Request.Form["EliminarServicioOtro"].Select(e => e == "true").ToList();
                /*verificar existencia servicio otro*/
                List<ServicioViewModel> verificarserviciootro = new List<ServicioViewModel>();

                for (int i = 0; i < idsservicios.Count; i++)
                {

                    var servicioViewModel = new ServicioViewModel
                    {

                        Idservicios = int.Parse(idsservicios[i]),

                        IdSegmento = int.Parse(idsegmentootro[i]),


                    };

                    verificarserviciootro.Add(servicioViewModel);

                }

                var Idserviciosocio = Request.Form["Idserviciosocio"];
                var Idproveedorsocio = Request.Form["Idproveedorsocio"];
                var montoserviciosociolist = Request.Form["montoserviciosocio"];
                var fechaserviciosocio = Request.Form["fechaserviciosocio"];
                var IdServicioRealsocio = Request.Form["IdServicioRealsocio"];
                var observacionserviciosocio = Request.Form["observacionserviciosocio"];
                var Estadosocio = Request.Form["Estadosocio"];
                var Tiposerviciosocio = Request.Form["Tiposerviciosocio"];
                var idsegmentosocio = Request.Form["idsegmentoserviciosocio"];
                var eliminarsocioservicio = Request.Form["EliminarServicioSocio"].Select(e => e == "true").ToList();

                /*verificar existencia servicio socio*/
                List<ServicioViewModel> verificarserviciosocio = new List<ServicioViewModel>();

                for (int i = 0; i < Idserviciosocio.Count; i++)
                {

                    var servicioViewModel = new ServicioViewModel
                    {

                        Idservicios = int.Parse(Idserviciosocio[i]),

                        IdSegmento = int.Parse(idsegmentosocio[i]),


                    };

                    verificarserviciosocio.Add(servicioViewModel);

                }



                var Idserviciohonorario = Request.Form["Idserviciohonorario"];
                var Idproveedorhonorario = Request.Form["Idproveedorhonorario"];
                var montoserviciohonorariolist = Request.Form["montoserviciohonorario"];
                var fechaserviciohonorario = Request.Form["fechaserviciohonorario"];
                var IdServicioRealhonorario = Request.Form["IdServicioRealhonorario"];
                var observacionserviciohonorario = Request.Form["observacionserviciohonorario"];
                var Estadohonorario = Request.Form["Estadohonorario"];
                var Tiposerviciohonorario = Request.Form["Tiposerviciohonorario"];
                var idsegmentohonorario = Request.Form["idsegmentoserviciohonorario"];
                var eliminarserviciohonorario = Request.Form["EliminarServicioHonorario"].Select(e => e == "true").ToList(); ;
                /*verificar existencia servicio honorarios*/
                List<ServicioViewModel> verificarserviciohonorario = new List<ServicioViewModel>();

                for (int i = 0; i < Idserviciohonorario.Count; i++)
                {

                    var servicioViewModel = new ServicioViewModel
                    {

                        Idservicios = int.Parse(Idserviciohonorario[i]),

                        IdSegmento = int.Parse(idsegmentohonorario[i]),


                    };

                    verificarserviciohonorario.Add(servicioViewModel);

                }



                var servicios = ProcesarServicios(Request.Form["Idservicio"].ToList(), Request.Form["Idproveedor"].ToList(), Request.Form["montoservicio"].ToList(), Request.Form["fechaservicio"].ToList(), Request.Form["IdServicioReal"].ToList(), Request.Form["observacionservicio"].ToList(), Request.Form["Estado"].ToList(), Request.Form["Tiposervicio"].ToList(), eliminarServicioOtro);
                var serviciossocios = ProcesarServicios(Request.Form["Idserviciosocio"].ToList(), Request.Form["Idproveedorsocio"].ToList(), Request.Form["montoserviciosocio"].ToList(), Request.Form["fechaserviciosocio"].ToList(), Request.Form["IdServicioRealsocio"].ToList(), Request.Form["observacionserviciosocio"].ToList(), Request.Form["Estadosocio"].ToList(), Request.Form["Tiposerviciosocio"].ToList(), eliminarsocioservicio);
                var servicioshonorarios = ProcesarServicios(Request.Form["Idserviciohonorario"].ToList(), Request.Form["Idproveedorhonorario"].ToList(), Request.Form["montoserviciohonorario"].ToList(), Request.Form["fechaserviciohonorario"].ToList(), Request.Form["IdServicioRealhonorario"].ToList(), Request.Form["observacionserviciohonorario"].ToList(), Request.Form["Estadohonorario"].ToList(), Request.Form["Tiposerviciohonorario"].ToList(), eliminarserviciohonorario);



                /*Gastos*/
                List<GastosRealesViewModel> gastos = new List<GastosRealesViewModel>();
                var idsgastos = Request.Form["idgastos"];
                var idproveedoresgastos = Request.Form["idproveedorgastos"];
                var idsegmentogastos = Request.Form["idsegmentogasto"];
                var montogastosList = Request.Form["montogasto"];
                var fechagastos = Request.Form["fechagasto"];
                var idgastoreal = Request.Form["IdGastoReal"];
                var observaciongasto = Request.Form["observaciongasto"];
                var estadogasto = Request.Form["EstadoGasto"];
                var eliminargasto = Request.Form["esEliminados"];

                for (int i = 0; i < idsgastos.Count; i++)
                {
                    var montogastoStr = montogastosList[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(montogastoStr))
                    {
                        montogastoStr = "0";
                    }
                    else
                    {
                        montogastoStr = montogastoStr.Replace(".", "");
                    }

                    decimal montogasto = decimal.Parse(montogastoStr);

                    int idgastoRealParsed = string.IsNullOrWhiteSpace(idgastoreal[i])
                                               ? 0
                                               : int.Parse(idgastoreal[i]);

                    DateTime fechaGastoParsed;
                    if (string.IsNullOrWhiteSpace(fechagastos[i]))
                    {
                        fechaGastoParsed = DateTime.Today;
                    }
                    else
                    {
                        fechaGastoParsed = DateTime.Parse(fechagastos[i]);
                    }
                    var gastoViewModel = new GastosRealesViewModel
                    {
                        IdGastoReal = idgastoRealParsed,
                        Idgasto = int.Parse(idsgastos[i]),
                        Idproveedor = int.Parse(idproveedoresgastos[i]),
                        Segmento = int.Parse(idsegmentogastos[i]),
                        Monto = montogasto,
                        Fecha = fechaGastoParsed,
                        Observacion = observaciongasto[i],
                        Estado = estadogasto[i],
                        EsEliminado = eliminargasto[i] == "true",
                    };

                    gastos.Add(gastoViewModel);


                }
                /*verificar existencia gasto*/
                List<GastoViewModel> verificargastos = new List<GastoViewModel>();

                for (int i = 0; i < idsgastos.Count; i++)
                {

                    var gastoViewModel = new GastoViewModel
                    {

                        Idgastos = int.Parse(idsgastos[i]),

                        IdSegmento = int.Parse(idsegmentogastos[i]),


                    };

                    verificargastos.Add(gastoViewModel);

                }



                await ejecucionService.GestorServiciosReales(idproyecto, servicios);
                await ejecucionService.GestorServiciosReales(idproyecto, serviciossocios);
                await ejecucionService.GestorServiciosReales(idproyecto, servicioshonorarios);
                await ejecucionService.GestorGastosReales(idproyecto, gastos);
                await ejecucionService.GestorGastosHH(idproyecto, gastosHH);
                await proyectoService.AgregarGastoProyectoeJECUCION(idproyecto, verificargastos);
                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarserviciootro);
                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarserviciosocio);
                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarserviciohonorario);
                return RedirectToAction("ForecastCostos", "EjecucionProyecto", new { id = idproyecto });
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Hubo un error al registrar/editar costos del proyecto:{e.Message}");
                TempData["ErrorMessage"] = "Hubo un error al Registrar/Editar costos del proyecto.";
                return RedirectToAction("ForecastCostos", "EjecucionProyecto", new { id = idproyecto });
            }
            
        }

        private List<ServiciosRealesViewModel> ProcesarServicios(List<string> ids, List<string> proveedores, List<string> montos, List<string> fechas, List<string> idsReales, List<string> observaciones, List<string> estados, List<string> tipos,List<bool> eliminar)
        {
            List<ServiciosRealesViewModel> servicios = new List<ServiciosRealesViewModel>();

            for (int i = 0; i < ids.Count; i++)
            {
           
                string montoStr = montos[i]?.ToString().Trim() ?? "0";
                montoStr = string.IsNullOrEmpty(montoStr) ? "0" : montoStr.Replace(".", "");
                decimal monto = decimal.TryParse(montoStr, out decimal result) ? result : 0;

               
                int idServicioReal = int.TryParse(idsReales[i], out int parsedIdServicioReal) ? parsedIdServicioReal : 0;

         
                DateTime fecha = DateTime.TryParse(fechas[i], out DateTime parsedFecha) ? parsedFecha : DateTime.Today;

               
                var servicioViewModel = new ServiciosRealesViewModel
                {
                    IdServicioReal = idServicioReal,
                    Idservicio = int.Parse(ids[i]),
                    Idproveedor = int.Parse(proveedores[i]),
                    Monto = monto,
                    Fecha = fecha,
                    Observacion = observaciones[i],
                    Estado = estados[i],
                    Tiposervicio = tipos[i],
                    EsEliminado = eliminar[i],
                };

                servicios.Add(servicioViewModel);
            }

            return servicios;
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerProveedoresServicios()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Servicio%")).OrderBy(p => p.Nombre)
            .ToListAsync();
            return Json(proveedores);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProveedoresGastos()
        {
            var proveedores = await context.Proveedores
            .Where(p => EF.Functions.Like(p.Tipo, "%Gasto%")).OrderBy(p => p.Nombre)
            .ToListAsync();
            return Json(proveedores);
        }


        [HttpGet]
        public async Task<IActionResult> RecuperarForecastProveedores(int? idproyecto)
        {
            var resultado = await ejecucionService.ObtenerForecastProveedores(idproyecto);
            return Json(resultado);
        }


        /*FLUJO CAJA*/
        public async Task<IActionResult> FlujoCajaProyecto(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var proyecto = await proyectoService.ObtenerProyectos(id,idcliente,nombre,idtipoempresa,statusproyecto,numproyecto,idtipologia,unidadneg,idccosto,idusuario);
            ViewBag.Proyecto = proyecto;          
            var flujocaja = await reporteService.ProcesarFlujoCajaPorMesAsync(id);
            return View(flujocaja);
        }
    }
}
