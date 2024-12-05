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
            var montousdlist = Request.Form["Montous"];
            var Tclist = Request.Form["Tc"];
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
              
                string montosusdStr = montousdlist[i]?.ToString().Trim() ?? "0";
                string montosclpStr = Montoclplist[i]?.ToString().Trim() ?? "0";
                string montosivaStr = Ivalist[i]?.ToString().Trim() ?? "0";
                string tcStr = Tclist[i]?.ToString().Trim() ?? "0";

                
                decimal.TryParse(montosusdStr.Replace(".", ""), out decimal montousd);
                decimal.TryParse(montosclpStr.Replace(".", ""), out decimal montoclp);
                decimal.TryParse(montosivaStr.Replace(".", ""), out decimal montoiva);
                decimal.TryParse(tcStr.Replace(".", ""), out decimal tc);

              
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
                    Montous = montousd,
                    Tc = tc,
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
            List<ServiciosRealesViewModel> servicios = new List<ServiciosRealesViewModel>();
            var idsservicios = Request.Form["Idservicio"];
            var idproveedores = Request.Form["Idproveedor"];
            var montoservicioList = Request.Form["montoservicio"];
            var fechaservicio = Request.Form["fechaservicio"];
            var idservicioreal = Request.Form["IdServicioReal"];
            var observacion = Request.Form["observacionservicio"];
            var estadoservicio = Request.Form["Estado"];
           
            for (int i = 0; i < idsservicios.Count; i++)
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
                };

                servicios.Add(servicioViewModel);
            }


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
            await ejecucionService.GestorGastosReales(idproyecto, gastos);
            await ejecucionService.GestorGastosHH(idproyecto, gastosHH);
            return RedirectToAction("ForecastCostos", "EjecucionProyecto", new {id = idproyecto });
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
