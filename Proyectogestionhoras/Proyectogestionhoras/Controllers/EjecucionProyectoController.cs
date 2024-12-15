using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Proyectogestionhoras.Models;
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
        private readonly PROYECTO_CONTROL_HORASContext context;
        public EjecucionProyectoController(ProyectoService proyectoService, FacturaService facturaService, EjecucionService ejecucionService, PROYECTO_CONTROL_HORASContext context)
        {
            this.proyectoService = proyectoService;
            this.facturaService = facturaService;
            this.ejecucionService = ejecucionService;
            this.context = context;
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
            var flujo = await ejecucionService.ObtenerFlujoCajaProyecto(id);
            var ingresos = await context.Ingresosreales.Where(x => x.Idproyecto == id).ToListAsync();
            ViewBag.Ingresos = ingresos;
            ViewBag.Proyecto = proyecto;
            ViewBag.Flujo = flujo;
            return View();
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
                        

            /*COSTOS SERVICIOS REALES*/
            /*List<ServiciosRealesViewModel> servicios = new List<ServiciosRealesViewModel>();*/
            var idsservicios = Request.Form["Idservicio"];
            var idproveedores = Request.Form["Idproveedor"];
            var montoservicioList = Request.Form["montoservicio"];
            var fechaservicio = Request.Form["fechaservicio"];
            var idservicioreal = Request.Form["IdServicioReal"];
            var observacion = Request.Form["observacionservicio"];
            var estadoservicio = Request.Form["Estado"];
            var tiposervicio = Request.Form["Tiposervicio"];
           
            /*for (int i = 0; i < idsservicios.Count; i++)
            {
                var montoservicioStr = montoservicioList[i]?.ToString().Trim() ?? ""; 
                if (string.IsNullOrEmpty(montoservicioStr))
                {
                    montoservicioStr = "0"; 
                }
                else
                {
                    montoservicioStr = montoservicioStr.Replace(".", ""); 
                }

                decimal montoservicio = decimal.Parse(montoservicioStr);

                int idServicioRealParsed = string.IsNullOrWhiteSpace(idservicioreal[i])
                                           ? 0
                                           : int.Parse(idservicioreal[i]);

                DateTime fechaServicioParsed;
                if (string.IsNullOrWhiteSpace(fechaservicio[i]))
                {
                    fechaServicioParsed = DateTime.Today;  
                }
                else
                {
                    fechaServicioParsed = DateTime.Parse(fechaservicio[i]);  
                }
                var servicioViewModel = new ServiciosRealesViewModel
                {
                    IdServicioReal = idServicioRealParsed, 
                    Idservicio = int.Parse(idsservicios[i]),
                    Idproveedor = int.Parse(idproveedores[i]),
                    Monto = montoservicio,
                    Fecha = fechaServicioParsed,
                    Observacion = observacion[i],
                    Estado = estadoservicio[i],
                    Tiposervicio = tiposervicio[i],
                };

                servicios.Add(servicioViewModel);
            }*/

            /*gastos socios*/
           
           /* List<ServiciosRealesViewModel> serviciossocios = new List<ServiciosRealesViewModel>();*/
            var Idserviciosocio = Request.Form["Idserviciosocio"];
            var Idproveedorsocio = Request.Form["Idproveedorsocio"];
            var montoserviciosociolist = Request.Form["montoserviciosocio"];
            var fechaserviciosocio = Request.Form["fechaserviciosocio"];
            var IdServicioRealsocio = Request.Form["IdServicioRealsocio"];
            var observacionserviciosocio = Request.Form["observacionserviciosocio"];
            var Estadosocio = Request.Form["Estadosocio"];
            var Tiposerviciosocio = Request.Form["Tiposerviciosocio"];

          /*  for (int i = 0; i < Idserviciosocio.Count; i++)
            {
                var montoservicioStr = montoserviciosociolist[i]?.ToString().Trim() ?? "";
                if (string.IsNullOrEmpty(montoservicioStr))
                {
                    montoservicioStr = "0";
                }
                else
                {
                    montoservicioStr = montoservicioStr.Replace(".", "");
                }

                decimal montoservicio = decimal.Parse(montoservicioStr);

                int idServicioRealParsed = string.IsNullOrWhiteSpace(IdServicioRealsocio[i])
                                           ? 0
                                           : int.Parse(IdServicioRealsocio[i]);

                DateTime fechaServicioParsed;
                if (string.IsNullOrWhiteSpace(fechaserviciosocio[i]))
                {
                    fechaServicioParsed = DateTime.Today;
                }
                else
                {
                    fechaServicioParsed = DateTime.Parse(fechaserviciosocio[i]);
                }
                var serviciosocioViewModel = new ServiciosRealesViewModel
                {
                    IdServicioReal = idServicioRealParsed,
                    Idservicio = int.Parse(Idserviciosocio[i]),
                    Idproveedor = int.Parse(Idproveedorsocio[i]),
                    Monto = montoservicio,
                    Fecha = fechaServicioParsed,
                    Observacion = observacionserviciosocio[i],
                    Estado = Estadosocio[i],
                    Tiposervicio = Tiposerviciosocio[i],
                };

                serviciossocios.Add(serviciosocioViewModel);
            }*/

            /*honorarios*/

           /* List<ServiciosRealesViewModel> servicioshonorarios = new List<ServiciosRealesViewModel>();*/
            var Idserviciohonorario = Request.Form["Idserviciohonorario"];
            var Idproveedorhonorario = Request.Form["Idproveedorhonorario"];
            var montoserviciohonorariolist = Request.Form["montoserviciohonorario"];
            var fechaserviciohonorario = Request.Form["fechaserviciohonorario"];
            var IdServicioRealhonorario = Request.Form["IdServicioRealhonorario"];
            var observacionserviciohonorario = Request.Form["observacionserviciohonorario"];
            var Estadohonorario = Request.Form["Estadohonorario"];
            var Tiposerviciohonorario = Request.Form["Tiposerviciohonorario"];

           /* for (int i = 0; i < Idserviciohonorario.Count; i++)
            {
                var montoservicioStr = montoserviciohonorariolist[i]?.ToString().Trim() ?? "";
                if (string.IsNullOrEmpty(montoservicioStr))
                {
                    montoservicioStr = "0";
                }
                else
                {
                    montoservicioStr = montoservicioStr.Replace(".", "");
                }

                decimal montoservicio = decimal.Parse(montoservicioStr);

                int idServicioRealParsed = string.IsNullOrWhiteSpace(IdServicioRealhonorario[i])
                                           ? 0
                                           : int.Parse(IdServicioRealhonorario[i]);

                DateTime fechaServicioParsed;
                if (string.IsNullOrWhiteSpace(fechaserviciohonorario[i]))
                {
                    fechaServicioParsed = DateTime.Today;
                }
                else
                {
                    fechaServicioParsed = DateTime.Parse(fechaserviciohonorario[i]);
                }
                var serviciohonorarioViewModel = new ServiciosRealesViewModel
                {
                    IdServicioReal = idServicioRealParsed,
                    Idservicio = int.Parse(Idserviciohonorario[i]),
                    Idproveedor = int.Parse(Idproveedorhonorario[i]),
                    Monto = montoservicio,
                    Fecha = fechaServicioParsed,
                    Observacion = observacionserviciohonorario[i],
                    Estado = Estadohonorario[i],
                    Tiposervicio = Tiposerviciohonorario[i],
                };

                servicioshonorarios.Add(serviciohonorarioViewModel);
            }*/
            var servicios = ProcesarServicios(Request.Form["Idservicio"].ToList(), Request.Form["Idproveedor"].ToList(), Request.Form["montoservicio"].ToList(), Request.Form["fechaservicio"].ToList(), Request.Form["IdServicioReal"].ToList(), Request.Form["observacionservicio"].ToList(), Request.Form["Estado"].ToList(), Request.Form["Tiposervicio"].ToList());
            var serviciossocios = ProcesarServicios(Request.Form["Idserviciosocio"].ToList(), Request.Form["Idproveedorsocio"].ToList(), Request.Form["montoserviciosocio"].ToList(), Request.Form["fechaserviciosocio"].ToList(), Request.Form["IdServicioRealsocio"].ToList(), Request.Form["observacionserviciosocio"].ToList(), Request.Form["Estadosocio"].ToList(), Request.Form["Tiposerviciosocio"].ToList());
            var servicioshonorarios = ProcesarServicios(Request.Form["Idserviciohonorario"].ToList(),Request.Form["Idproveedorhonorario"].ToList(),Request.Form["montoserviciohonorario"].ToList(),Request.Form["fechaserviciohonorario"].ToList(),Request.Form["IdServicioRealhonorario"].ToList(),Request.Form["observacionserviciohonorario"].ToList(),Request.Form["Estadohonorario"].ToList(),Request.Form["Tiposerviciohonorario"].ToList()
);



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
                };

                gastos.Add(gastoViewModel);


            }
            

            await ejecucionService.GestorServiciosReales(idproyecto, servicios);
            await ejecucionService.GestorServiciosReales(idproyecto, serviciossocios);
            await ejecucionService.GestorServiciosReales(idproyecto, servicioshonorarios);
            await ejecucionService.GestorGastosReales(idproyecto, gastos);
            await ejecucionService.GestorGastosHH(idproyecto, gastosHH);
            return RedirectToAction("ForecastCostos", "EjecucionProyecto", new {id = idproyecto });
        }

        private List<ServiciosRealesViewModel> ProcesarServicios(List<string> ids, List<string> proveedores, List<string> montos, List<string> fechas, List<string> idsReales, List<string> observaciones, List<string> estados, List<string> tipos)
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
            var flujocajaingreso = await ejecucionService.ObtenerFlujoCajaProyecto(id);
            var flujocajaegreso = await ejecucionService.ObtenerEgresoFlujoCajaProyecto(id);
            var flujocajaegresosservicios = await ejecucionService.ObtenerEgresoServiciosFlujoCajaProyecto(id);
            var flujocajaegresosgastos = await ejecucionService.ObtenerEgresoGastosFlujoCajaProyecto(id);
            ViewBag.flujocajaprueba = flujocajaegresosservicios;
            ViewBag.flujogastos = flujocajaegresosgastos;
            var groupedServicios = flujocajaegresosservicios
                                    .GroupBy(s => s.ServicioNombre)
                                    .Select(g => new
                                    {
                                        ServicioNombre = g.Key,
                                        Servicios = g.ToList()
                                    })
                                    .ToList();

            var groupedgastos = flujocajaegresosgastos
                                    .GroupBy(s => s.GastoNombre)
                                    .Select(g => new
                                    {
                                        GastosNombres = g.Key,
                                        Gastos = g.ToList()
                                    })
                                    .ToList();


            var querysocio = from p in context.Proyectos
                        join up in context.UsuarioProyectos on p.Id equals up.IdProyecto
                        join s in context.Segmentos on up.Idsegmento equals s.Id
                        join c in context.Cuenta on s.IdCuenta equals c.Id
                        join u in context.Usuarios on up.IdUsuario equals u.Id
                        join r in context.Recursos on u.IdRecurso equals r.Id
                        where p.Id == id && r.NombreRecurso == "Socio"
                        select new
                        {
                            c.Cuenta,
                            c.Idcuenta
                        };

            var querystaff = from p in context.Proyectos
                             join up in context.UsuarioProyectos on p.Id equals up.IdProyecto
                             join s in context.Segmentos on up.Idsegmento equals s.Id
                             join c in context.Cuenta on s.IdCuenta equals c.Id
                             join u in context.Usuarios on up.IdUsuario equals u.Id
                             join r in context.Recursos on u.IdRecurso equals r.Id
                             where p.Id == id && r.NombreRecurso == "Staff"
                             select new
                             {
                                 c.Cuenta,
                                 c.Idcuenta
                             };

            var querysconsultor = from p in context.Proyectos
                             join up in context.UsuarioProyectos on p.Id equals up.IdProyecto
                             join s in context.Segmentos on up.Idsegmento equals s.Id
                             join c in context.Cuenta on s.IdCuenta equals c.Id
                             join u in context.Usuarios on up.IdUsuario equals u.Id
                             join r in context.Recursos on u.IdRecurso equals r.Id
                             where p.Id == id && r.NombreRecurso == "Consultor Externo"
                                  select new
                             {
                                 c.Cuenta,
                                 c.Idcuenta
                             };
           


            ViewBag.Proyecto = proyecto;
            ViewBag.Flujo = flujocajaingreso;
            ViewBag.Egresos = flujocajaegreso;
            ViewBag.Servicios = groupedServicios;
           ViewBag.Gastos = groupedgastos;
            ViewBag.CuentaSocio = querysocio;
            ViewBag.CuentaStaff = querystaff;
            ViewBag.CuentaConsultor = querysconsultor;






            return View();
        }
    }
}
