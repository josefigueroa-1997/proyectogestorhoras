using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Newtonsoft.Json;
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
            var idsession = HttpContext.Session.GetInt32("id");
            if (idsession.HasValue)
            {
                var proyectos = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
                ViewBag.Proyectos = proyectos;
                return View();

            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult RedirigirActividad()
        {
            int idproyecto = int.Parse(Request.Form["idproyecto"].ToString());
            HttpContext.Session.SetInt32("numproyecto", idproyecto);
            return RedirectToAction("SeleccionarActividad", "EjecucionProyecto");
        }
        public IActionResult SeleccionarActividad()
        {

            var idsession = HttpContext.Session.GetInt32("id");
            if (idsession.HasValue)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");

        }

        /*FORECAST INGRESOS*/
        public async Task<IActionResult> ForecastIngreso(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var idsession = HttpContext.Session.GetInt32("id");
            if (idsession.HasValue)
            {
                var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
                var ingresos = await context.Ingresosreales.Where(x => x.Idproyecto == id).ToListAsync();
                var flujocaja = await reporteService.ProcesarFlujoCajaPorMesAsync(id);
                ViewBag.Ingresos = ingresos;
                ViewBag.Proyecto = proyecto;
                return View(flujocaja);

            }
            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public async Task<IActionResult> RegistrarIngresos(int idproyecto)
        {
            try
            {
                List<IngresoViewModel> ingresos = new List<IngresoViewModel>();

                var numdocumento = Request.Form["Numdocumento"];
                var fechapago = Request.Form["FechaPago"];
                var fechaemision = Request.Form["FechaEmision"];

                var Montoclplist = Request.Form["Montoclp"];
                var Ivalist = Request.Form["Iva"];
                var Estado = Request.Form["Estado"];
                var Idcuenta = Request.Form["Idcuenta"];
                var Observacion = Request.Form["Observacion"];
                var idingresoreal = Request.Form["IdIngresoreal"];
                var esliminado = Request.Form["esEliminados"];


                for (int i = 0; i < numdocumento.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(numdocumento[i]))
                    {
                        continue;
                    }


                    string montosclpStr = Montoclplist[i]?.ToString().Trim() ?? "0";
                    string montosivaStr = Ivalist[i]?.ToString().Trim() ?? "0";




                    decimal.TryParse(montosclpStr.Replace(".", ""), out decimal montoclp);
                    decimal.TryParse(montosivaStr.Replace(".", ""), out decimal montoiva);



                    int.TryParse(idingresoreal[i]?.ToString(), out int idIngresoRealParsed);
                    int.TryParse(Idcuenta[i]?.ToString(), out int idCuentaParsed);


                    DateTime fechaemisionParsed = DateTime.TryParse(fechaemision[i], out DateTime tempDate)
                        ? tempDate
                        : DateTime.Today;


                    var ingresoViewModel = new IngresoViewModel
                    {
                        IdIngresoreal = idIngresoRealParsed,
                        Numdocumento = numdocumento[i],
                        FechaEmision = fechaemisionParsed,
                        FechaPago = DateTime.TryParse(fechapago[i], out DateTime fecha) ? (DateTime?)fecha : null,

                        Montoclp = montoclp,
                        Iva = montoiva,
                        Estado = Estado[i],

                        Idcuenta = idCuentaParsed,
                        Observacion = Observacion[i],
                        EsEliminado = esliminado[i] == "true",
                    };

                    ingresos.Add(ingresoViewModel);
                }
                await proyectoService.GestorFechaModificacionProyecto(idproyecto);
                await ejecucionService.GestorIngresos(idproyecto, ingresos);
                TempData["SuccessMessageIngresos"] = "Los ingresos del proyecto se han registrado y actualizado correctamente.";
                return RedirectToAction("ForecastIngreso", "EjecucionProyecto", new { id = idproyecto });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al registrar/editar costos del proyecto:{e.Message}");
                TempData["ErrorMessageIngresos"] = "Hubo un error al Registrar/Editar ingresos del proyecto.";
                return RedirectToAction("ForecastIngreso", "EjecucionProyecto", new { id = idproyecto });
            }

        }

        [HttpGet]
        public async Task<IActionResult> RecuperarForecastIngresos()
        {
            var resultado = await ejecucionService.ObtenerForecastIngresos();
            return Json(resultado);
        }

        /*FORECAST COSTOS*/


        public async Task<IActionResult> PagosDistribucionHH(int? idproyecto, int? estado)
        {
            var idsession = HttpContext.Session.GetInt32("id");
            if (idsession.HasValue)
            {
                var distribucion = await ejecucionService.ObtenerDistribucionHH(idproyecto, estado);
                return View(distribucion);

            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> CargarGastosReales(int idproyecto)
        {
           
            var resultado = await ejecucionService.ObtenerGastosReales(idproyecto);
           

           
            return Json(resultado);
        }


        [HttpGet]
        public async Task<IActionResult> CargarServiciosReales(int idproyecto, string tipo)
        {
            var resultado = await ejecucionService.ObtenerServiciosReales(idproyecto, tipo);
          
            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosResumen(int idProyecto)
        {
            try
            {
                
                var serviciosProyecto = await proyectoService.ObtenerServiciosProyecto(idProyecto);
                var gastosProyecto = await proyectoService.ObtenerGastosProyectos(idProyecto);

               
                var serviciosejecucion = await context.Serviciosejecucions
                    .Where(s => s.Idproyecto == idProyecto)
                    .ToListAsync();

                var gastosejecucion = await context.Gastosejecucions
                    .Where(s => s.Idproyecto == idProyecto)
                    .ToListAsync();

                
                var serviciosreales = serviciosejecucion
                    .GroupBy(x => new { x.Idservicio, x.Estado })
                    .Select(g => new
                    {
                        Idservicio = g.Key.Idservicio,
                        Estado = g.Key.Estado,
                        TotalMonto = g.Sum(x => x.Monto)
                    }).ToList();

                var gastosreales = gastosejecucion
                    .GroupBy(x => new { x.Idgasto, x.Estado })
                    .Select(g => new
                    {
                        Idgasto = g.Key.Idgasto,
                        Estado = g.Key.Estado,
                        TotalMonto = g.Sum(x => x.Monto)
                    }).ToList();

                // Diccionarios de totales
                var serviciosTotalesPagados = serviciosreales
                    .Where(x => x.Estado == "Pagada")
                    .GroupBy(x => x.Idservicio)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var serviciosTotalesForecast = serviciosreales
                    .Where(x => x.Estado == "Forecast")
                    .GroupBy(x => x.Idservicio)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var gastosTotalesPagados = gastosreales
                    .Where(x => x.Estado == "Pagada")
                    .GroupBy(x => x.Idgasto)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var gastosTotalesForecast = gastosreales
                    .Where(x => x.Estado == "Forecast")
                    .GroupBy(x => x.Idgasto)
                    .ToDictionary(g => g.Key, g => g.First().TotalMonto);

                var jsonResult = JsonConvert.SerializeObject(new
                {
                    ServiciosProyectos = serviciosProyecto,
                    ServiciosTotalesPagados = serviciosTotalesPagados,
                    ServiciosTotalesForecast = serviciosTotalesForecast,
                    GastosProyectos = gastosProyecto,
                    GastosTotalesPagados = gastosTotalesPagados,
                    GastosTotalesForecast = gastosTotalesForecast
                });

                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR EJECUCION: " + ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }





        public async Task<IActionResult> EgresosProyectos(int? idproyecto)
        {

            var proyecto = await context.Proyectos.Where(p => p.Id == idproyecto).ToListAsync();
            ViewBag.Proyecto = proyecto;

            var gastoshh = await ejecucionService.ObtenerDistribucionHH(idproyecto, null);
            ViewBag.Idcuentasocio = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Idcuentasocio).FirstOrDefaultAsync();
            ViewBag.cuentasocio = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Cuentasocio).FirstOrDefaultAsync();
            ViewBag.costohhsocio = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Costosocio).FirstOrDefaultAsync();
            ViewBag.Idcuentastaff = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Idcuentastaff).FirstOrDefaultAsync();
            ViewBag.cuentastaff = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Cuentastaff).FirstOrDefaultAsync();
            ViewBag.costohhstaff = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == idproyecto).Select(hc => hc.Costostaff).FirstOrDefaultAsync();
            var datosgastosrecursos = await context.Gastoshhhejecucions.Where(g => g.Idproyecto == idproyecto).ToListAsync();
           

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarServicios()
        {
            var idproyecto = int.Parse(Request.Form["idproyecto"].ToString());
            try
            {
                /*COSTOS SERVICIOS base de datos*/
                var serviciosantiguos = Request.Form["Idservicio"];
                var eliminarServicioOtro = Request.Form["EliminarServicioOtro"].Select(e => e == "true").ToList();

                var servicios = ProcesarServicios(Request.Form["Idservicio"].ToList(), Request.Form["Idproveedorservicio"].ToList(), Request.Form["montoservicio"].ToList(), Request.Form["fechaservicio"].ToList(), Request.Form["IdServicioReal"].ToList(), Request.Form["observacionservicio"].ToList(), Request.Form["estadoservicio"].ToList(), Request.Form["Tiposervicio"].ToList(), eliminarServicioOtro);


                var eliminarServicioOtronuevo = Request.Form["EliminarServicioOtroNuevo"].Select(e => e == "true").ToList();

                var serviciosnuevos = ProcesarServicios(Request.Form["idservicionuevos"].ToList(), Request.Form["idproveedorgastosnuevos"].ToList(), Request.Form["montoserviciosnuevos"].ToList(), Request.Form["fechaserviciosnuevos"].ToList(), Request.Form["IdServicioRealnuevos"].ToList(), Request.Form["observacionservicionuevos"].ToList(), Request.Form["estadoserviciosnuevos"].ToList(), Request.Form["Tiposervicionuevo"].ToList(), eliminarServicioOtronuevo);


                var idservicionuevos = Request.Form["idservicionuevos"];


                await ejecucionService.GestorServiciosReales(idproyecto, servicios);
                await ejecucionService.GestorServiciosReales(idproyecto, serviciosnuevos);


                List<ServicioViewModel> verificarservicioantiguos = new List<ServicioViewModel>();

                for (int i = 0; i < serviciosantiguos.Count; i++)
                {

                    var servicioViewModel = new ServicioViewModel
                    {

                        Idservicios = int.Parse(serviciosantiguos[i]),



                    };

                    verificarservicioantiguos.Add(servicioViewModel);

                }

                List<ServicioViewModel> verificarservicionuevos = new List<ServicioViewModel>();

                for (int i = 0; i < idservicionuevos.Count; i++)
                {

                    var servicioViewModel = new ServicioViewModel
                    {

                        Idservicios = int.Parse(idservicionuevos[i]),



                    };

                    verificarservicionuevos.Add(servicioViewModel);

                }


                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarservicioantiguos);
                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarservicionuevos);
                TempData["SuccessMessageServicios"] = "Los Servicios del proyecto se han registrado y actualizado correctamente.";
                return Redirect($"{Url.Action("EgresosProyectos", "EjecucionProyecto", new { idproyecto })}#section-servicios");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al registrar seriviocs: {e.Message}");
                TempData["ErrorMessageServicios"] = "Error al registrar los gastos del proyecto.";
                return Redirect($"{Url.Action("EgresosProyectos", "EjecucionProyecto", new { idproyecto })}#section-servicios");
            }
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarGastos()
        {
            var idproyecto = int.Parse(Request.Form["idproyecto"].ToString());
            try
            {
                /*Gastos Nuevos*/

                List<GastosRealesViewModel> gastosnuevos = new List<GastosRealesViewModel>();
                var idsgastosnuevos = Request.Form["idgastosnuevos"];
                var idproveedoresgastosnuevos = Request.Form["idproveedorgastosnuevos"];
                var montogastosListnuevos = Request.Form["montogastonuevos"];
                var fechagastosnuevos = Request.Form["fechagastonuevos"];
                var idgastorealnuevos = Request.Form["IdGastoRealnuevos"];
                var observaciongastonuevos = Request.Form["observaciongastonuevos"];
                var estadogastonuevos = Request.Form["estadogastonuevos"];
                var eliminargastonuevos = Request.Form["esEliminadosnuevos"];


                for (int i = 0; i < idsgastosnuevos.Count; i++)
                {
                    var montogastoStr = montogastosListnuevos[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(montogastoStr))
                    {
                        montogastoStr = "0";
                    }
                    else
                    {
                        montogastoStr = montogastoStr.Replace(".", "");
                    }

                    decimal montogasto = decimal.Parse(montogastoStr);

                    int idgastoRealParsed = string.IsNullOrWhiteSpace(idgastorealnuevos[i])
                                               ? 0
                                               : int.Parse(idgastorealnuevos[i]);

                    DateTime fechaGastoParsed;
                    if (string.IsNullOrWhiteSpace(fechagastosnuevos[i]))
                    {
                        fechaGastoParsed = DateTime.Today;
                    }
                    else
                    {
                        fechaGastoParsed = DateTime.Parse(fechagastosnuevos[i]);
                    }
                    var gastoViewModel = new GastosRealesViewModel
                    {
                        IdGastoReal = idgastoRealParsed,
                        Idgasto = int.Parse(idsgastosnuevos[i]),
                        Idproveedor = int.Parse(idproveedoresgastosnuevos[i]),

                        Monto = montogasto,
                        Fecha = fechaGastoParsed,
                        Observacion = observaciongastonuevos[i],
                        Estado = estadogastonuevos[i],

                        EsEliminado = eliminargastonuevos[i] == "true",
                    };

                    gastosnuevos.Add(gastoViewModel);


                }



                /*Gastos existentes*/
                List<GastosRealesViewModel> gastos = new List<GastosRealesViewModel>();
                var idsgastos = Request.Form["idgastos"];
                var idproveedoresgastos = Request.Form["idproveedorgastos"];
                var montogastosList = Request.Form["montogasto"];
                var fechagastos = Request.Form["fechagasto"];
                var idgastoreal = Request.Form["IdGastoReal"];
                var observaciongasto = Request.Form["observaciongasto"];
                var estadogasto = Request.Form["estadogasto"];
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

                        Monto = montogasto,
                        Fecha = fechaGastoParsed,
                        Observacion = observaciongasto[i],
                        Estado = estadogasto[i],

                        EsEliminado = eliminargasto[i] == "true",
                    };

                    gastos.Add(gastoViewModel);


                }
                await ejecucionService.GestorGastosReales(idproyecto, gastos);
                await ejecucionService.GestorGastosReales(idproyecto, gastosnuevos);


                /*verificar existencia gasto antiguos*/
                List<GastoViewModel> verificargastos = new List<GastoViewModel>();

                for (int i = 0; i < idsgastos.Count; i++)
                {

                    var gastoViewModel = new GastoViewModel
                    {

                        Idgastos = int.Parse(idsgastos[i]),




                    };

                    verificargastos.Add(gastoViewModel);

                }

                /*verificar existencia gasto antiguos*/
                List<GastoViewModel> verificargastosnuevos = new List<GastoViewModel>();

                for (int i = 0; i < idsgastosnuevos.Count; i++)
                {

                    var gastoViewModel = new GastoViewModel
                    {

                        Idgastos = int.Parse(idsgastosnuevos[i]),




                    };

                    verificargastosnuevos.Add(gastoViewModel);

                }

                await proyectoService.AgregarGastoProyectoeJECUCION(idproyecto, verificargastos);
                await proyectoService.AgregarGastoProyectoeJECUCION(idproyecto, verificargastosnuevos);

                TempData["SuccessMessageGastos"] = "Los Gastos del proyecto se han registrado y actualizado correctamente.";
                return Redirect($"{Url.Action("EgresosProyectos", "EjecucionProyecto", new { idproyecto })}#section-gastos");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al registrar gastos: {ex.Message}");
                TempData["ErrorMessageGastos"] = "Error al registrar los gastos del proyecto.";
                return Redirect($"{Url.Action("EgresosProyectos", "EjecucionProyecto", new { idproyecto })}#section-gastos");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarHH()
        {
            var idproyecto = int.Parse(Request.Form["idproyecto"].ToString());
            try
            {
                /*gastos socios*/

                var idgastohhsocios = Request.Form["IdGastoHH"];
                var tiporecursosocioo = Request.Form["Tiporecurso"];
                var messocio = Request.Form["Mes"];
                var aniosocio = Request.Form["Anio"];
                var subtotalsociolist = Request.Form["Subtotal"];
                var hhtotalessocioslist = Request.Form["HHtotales"];
                var fechapagosocio = Request.Form["Fechapago"];
                var reajustesocioolist = Request.Form["Reajuste"];
                var montosociolist = Request.Form["Monto"];
                var observacionsocio = Request.Form["Observacion"];
                var costounitarioosociolist = Request.Form["CostoUnitario"];



                List<GastosHHViewModel> gastosocioo = new List<GastosHHViewModel>();

                for (int i = 0; i < idgastohhsocios.Count; i++)
                {
                    var subtotalsocioStr = subtotalsociolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(subtotalsocioStr))
                    {
                        subtotalsocioStr = "0";
                    }
                    else
                    {
                        subtotalsocioStr = subtotalsocioStr.Replace(".", "");
                    }

                    decimal subtotalsocio = decimal.Parse(subtotalsocioStr);


                    var reajustesocioStr = reajustesocioolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(reajustesocioStr))
                    {
                        reajustesocioStr = "0";
                    }
                    else
                    {
                        reajustesocioStr = reajustesocioStr.Replace(".", "");
                    }



                    decimal reajustesocio = decimal.Parse(reajustesocioStr);


                    var montosocioStr = montosociolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(montosocioStr))
                    {
                        montosocioStr = "0";
                    }
                    else
                    {
                        montosocioStr = montosocioStr.Replace(".", "");
                    }



                    decimal montosocio = decimal.Parse(montosocioStr);


                    var hhtotalessocioStr = hhtotalessocioslist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(hhtotalessocioStr))
                    {
                        hhtotalessocioStr = "0";
                    }
                    else
                    {
                        hhtotalessocioStr = hhtotalessocioStr.Replace(".", "");
                    }



                    decimal hhtotalessocios = decimal.Parse(hhtotalessocioStr);


                    var costounitariosocioStr = costounitarioosociolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(costounitariosocioStr))
                    {
                        costounitariosocioStr = "0";
                    }
                    else
                    {
                        costounitariosocioStr = costounitariosocioStr.Replace(".", "");
                    }

                    decimal costounitariosocio = decimal.Parse(costounitariosocioStr);



                    int idgastohhsocioRealParsed = string.IsNullOrWhiteSpace(idgastohhsocios[i])
                                               ? 0
                                               : int.Parse(idgastohhsocios[i]);



                    DateTime? fechapagoParsed;
                    if (string.IsNullOrWhiteSpace(fechapagosocio[i]))
                    {
                        fechapagoParsed = null;
                    }
                    else
                    {
                        fechapagoParsed = DateTime.Parse(fechapagosocio[i]);
                    }
                    var gastohhViewModel = new GastosHHViewModel
                    {
                        IdGastoHH = idgastohhsocioRealParsed,
                        Tiporecurso = tiporecursosocioo[i],
                        Mes = int.Parse(messocio[i]),
                        Anio = int.Parse(aniosocio[i]),
                        Fechapago = fechapagoParsed,
                        Monto = montosocio,
                        HHtotales = hhtotalessocios,
                        Observacion = observacionsocio[i],
                        Subtotal = subtotalsocio,
                        Reajuste = reajustesocio,
                        costonitario = costounitariosocio,
                    };

                    gastosocioo.Add(gastohhViewModel);


                }

                var gastohhsocio = ProcesarGastosHH(Request.Form, "Idhhsocio", "recursohhsocio", "montohhsocio", "hhsocio", "fechahhsocio", "observacionhhsocio", "estadohhsocio", "EliminarhhSocio", "costohhsocio");


                /*gastos staff*/

                var idgastohhstaff = Request.Form["IdGastoHHstaff"];
                var tiporecursostaff = Request.Form["Tiporecursostaff"];
                var messtaff = Request.Form["Messtaff"];
                var aniostaff = Request.Form["Aniostaff"];
                var subtotalstafflist = Request.Form["Subtotalstaff"];
                var hhtotalesstafflist = Request.Form["HHtotalesstaff"];
                var fechapagostaff = Request.Form["Fechapagostaff"];
                var reajustestafflist = Request.Form["Reajustestaff"];
                var montostafflist = Request.Form["Montostaff"];
                var observacionstaff = Request.Form["Observacionstaff"];
                var costounitariostafflist = Request.Form["CostoUnitarioStaff"];





                List<GastosHHViewModel> gastostaff = new List<GastosHHViewModel>();

                for (int i = 0; i < idgastohhstaff.Count; i++)
                {
                    var subtotalstaffStr = subtotalstafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(subtotalstaffStr))
                    {
                        subtotalstaffStr = "0";
                    }
                    else
                    {
                        subtotalstaffStr = subtotalstaffStr.Replace(".", "");
                    }

                    decimal subtotalstaff = decimal.Parse(subtotalstaffStr);


                    var reajustestaffStr = reajustestafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(reajustestaffStr))
                    {
                        reajustestaffStr = "0";
                    }
                    else
                    {
                        reajustestaffStr = reajustestaffStr.Replace(".", "");
                    }



                    decimal reajustestaff = decimal.Parse(reajustestaffStr);


                    var montostaffStr = montostafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(montostaffStr))
                    {
                        montostaffStr = "0";
                    }
                    else
                    {
                        montostaffStr = montostaffStr.Replace(".", "");
                    }



                    decimal montostaff = decimal.Parse(montostaffStr);


                    var hhtotalesstaffStr = hhtotalesstafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(hhtotalesstaffStr))
                    {
                        hhtotalesstaffStr = "0";
                    }
                    else
                    {
                        hhtotalesstaffStr = hhtotalesstaffStr.Replace(".", "");
                    }



                    decimal hhtotalesstaff = decimal.Parse(hhtotalesstaffStr);


                    var costounitariostaffStr = costounitariostafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(costounitariostaffStr))
                    {
                        costounitariostaffStr = "0";
                    }
                    else
                    {
                        costounitariostaffStr = costounitariostaffStr.Replace(".", "");
                    }

                    decimal costounitariostaff = decimal.Parse(costounitariostaffStr);



                    int idgastohhstaffRealParsed = string.IsNullOrWhiteSpace(idgastohhstaff[i])
                                               ? 0
                                               : int.Parse(idgastohhstaff[i]);



                    DateTime? fechapagostaffParsed;
                    if (string.IsNullOrWhiteSpace(fechapagostaff[i]))
                    {
                        fechapagostaffParsed = null;
                    }
                    else
                    {
                        fechapagostaffParsed = DateTime.Parse(fechapagostaff[i]);
                    }
                    var gastohhstaffViewModel = new GastosHHViewModel
                    {
                        IdGastoHH = idgastohhstaffRealParsed,
                        Tiporecurso = tiporecursostaff[i],
                        Mes = int.Parse(messtaff[i]),
                        Anio = int.Parse(aniostaff[i]),
                        Fechapago = fechapagostaffParsed,
                        Monto = montostaff,
                        HHtotales = hhtotalesstaff,
                        Observacion = observacionstaff[i],
                        Subtotal = subtotalstaff,
                        Reajuste = reajustestaff,
                        costonitario = costounitariostaff,
                    };

                    gastostaff.Add(gastohhstaffViewModel);


                }
                var gastohhstaff = ProcesarGastosHH(
                    Request.Form,
                    "Idhhstaff",
                    "recursohhstaff",
                    "montohhstaff",
                    "hhstaff",
                    "fechahhstaff",
                    "observacionhhstaff",
                    "estadohhstaff",
                    "EliminarhhStaff",
                    "costohhstaff"
                );

                await ejecucionService.GestorGastosHH(idproyecto, gastostaff);
                await ejecucionService.GestorGastosHH(idproyecto, gastosocioo);
                await ejecucionService.GestorGastosHH(idproyecto, gastohhsocio);
                await ejecucionService.GestorGastosHH(idproyecto, gastohhstaff);

                TempData["SuccessMessage"] = "Los Registros hh del proyecto se han registrado y actualizado correctamente.";
                return Redirect($"{Url.Action("EgresosProyectos", "EjecucionProyecto", new { idproyecto })}#section-hh");

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
                Debug.WriteLine(ex.InnerException);
                TempData["ErrorMessage"] = "Error al registrar/actualizar los registros hh del proyecto.";
                return Redirect($"{Url.Action("EgresosProyectos", "EjecucionProyecto", new { idproyecto })}#section-hh");

            }

        }


        public async Task<IActionResult> ForecastCostos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
        {
            var idsession = HttpContext.Session.GetInt32("id");
            if (idsession.HasValue)
            {
                var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
                var serviciosejecucion = await context.Serviciosejecucions.Where(s => s.Idproyecto == id).OrderBy(s => s.Estado != "Forecast").ThenBy(s => s.Fecha).ToListAsync();
                var gastosejecucion = await context.Gastosejecucions.Where(s => s.Idproyecto == id).OrderBy(s => s.Estado != "Forecast").ThenBy(s => s.Fecha).ToListAsync();
                var servicios = await GetServicios();
                var gastos = await GetGastos();
                var proveedoresservicios = await GetProveedoresServicios();
                var proveedoresgastos = await GetProveedoresGastos();
                var gastoshh = await ejecucionService.ObtenerDistribucionHH(id, null);
                ViewBag.Idcuentasocio = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == id).Select(hc => hc.Idcuentasocio).FirstOrDefaultAsync();
                ViewBag.cuentasocio = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == id).Select(hc => hc.Cuentasocio).FirstOrDefaultAsync();
                ViewBag.costohhsocio = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == id).Select(hc => hc.Costosocio).FirstOrDefaultAsync();
                ViewBag.Idcuentastaff = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == id).Select(hc => hc.Idcuentastaff).FirstOrDefaultAsync();
                ViewBag.cuentastaff = await context.Historialcuentasproyectos.Where(hc => hc.Idproyecto == id).Select(hc => hc.Cuentastaff).FirstOrDefaultAsync();
                ViewBag.costohhstaff = await context.HistorialCostosProyectos.Where(hc => hc.Idproyecto == id).Select(hc => hc.Costostaff).FirstOrDefaultAsync();
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
                                    group gasejecucion by new { gasejecucion.Idgasto, gasejecucion.Estado } into grupo
                                    select new
                                    {
                                        IdGasto = grupo.Key.Idgasto,
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
            return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> RegistrarCostos(int idproyecto)
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
                var eliminarserviciohonorario = Request.Form["EliminarServicioHonorario"].Select(e => e == "true").ToList();

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

                /*gastos socios*/

                var idgastohhsocios = Request.Form["IdGastoHH"];
                var tiporecursosocioo = Request.Form["Tiporecurso"];
                var messocio = Request.Form["Mes"];
                var aniosocio = Request.Form["Anio"];
                var subtotalsociolist = Request.Form["Subtotal"];
                var hhtotalessocioslist = Request.Form["HHtotales"];
                var fechapagosocio = Request.Form["Fechapago"];
                var reajustesocioolist = Request.Form["Reajuste"];
                var montosociolist = Request.Form["Monto"];
                var observacionsocio = Request.Form["Observacion"];
                var costounitarioosociolist = Request.Form["CostoUnitario"];



                List<GastosHHViewModel> gastosocioo = new List<GastosHHViewModel>();

                for (int i = 0; i < idgastohhsocios.Count; i++)
                {
                    var subtotalsocioStr = subtotalsociolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(subtotalsocioStr))
                    {
                        subtotalsocioStr = "0";
                    }
                    else
                    {
                        subtotalsocioStr = subtotalsocioStr.Replace(".", "");
                    }

                    decimal subtotalsocio = decimal.Parse(subtotalsocioStr);


                    var reajustesocioStr = reajustesocioolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(reajustesocioStr))
                    {
                        reajustesocioStr = "0";
                    }
                    else
                    {
                        reajustesocioStr = reajustesocioStr.Replace(".", "");
                    }



                    decimal reajustesocio = decimal.Parse(reajustesocioStr);


                    var montosocioStr = montosociolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(montosocioStr))
                    {
                        montosocioStr = "0";
                    }
                    else
                    {
                        montosocioStr = montosocioStr.Replace(".", "");
                    }



                    decimal montosocio = decimal.Parse(montosocioStr);


                    var hhtotalessocioStr = hhtotalessocioslist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(hhtotalessocioStr))
                    {
                        hhtotalessocioStr = "0";
                    }
                    else
                    {
                        hhtotalessocioStr = hhtotalessocioStr.Replace(".", "");
                    }



                    decimal hhtotalessocios = decimal.Parse(hhtotalessocioStr);


                    var costounitariosocioStr = costounitarioosociolist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(costounitariosocioStr))
                    {
                        costounitariosocioStr = "0";
                    }
                    else
                    {
                        costounitariosocioStr = costounitariosocioStr.Replace(".", "");
                    }

                    decimal costounitariosocio = decimal.Parse(costounitariosocioStr);



                    int idgastohhsocioRealParsed = string.IsNullOrWhiteSpace(idgastohhsocios[i])
                                               ? 0
                                               : int.Parse(idgastohhsocios[i]);



                    DateTime? fechapagoParsed;
                    if (string.IsNullOrWhiteSpace(fechapagosocio[i]))
                    {
                        fechapagoParsed = null;
                    }
                    else
                    {
                        fechapagoParsed = DateTime.Parse(fechapagosocio[i]);
                    }
                    var gastohhViewModel = new GastosHHViewModel
                    {
                        IdGastoHH = idgastohhsocioRealParsed,
                        Tiporecurso = tiporecursosocioo[i],
                        Mes = int.Parse(messocio[i]),
                        Anio = int.Parse(aniosocio[i]),
                        Fechapago = fechapagoParsed,
                        Monto = montosocio,
                        HHtotales = hhtotalessocios,
                        Observacion = observacionsocio[i],
                        Subtotal = subtotalsocio,
                        Reajuste = reajustesocio,
                        costonitario = costounitariosocio,
                    };

                    gastosocioo.Add(gastohhViewModel);


                }

                /*hh forecast socios*/


                var gastohhsocio = ProcesarGastosHH(Request.Form, "Idhhsocio", "recursohhsocio", "montohhsocio", "hhsocio", "fechahhsocio", "observacionhhsocio", "estadohhsocio", "EliminarhhSocio", "costohhsocio");


                /*gastos staff*/

                var idgastohhstaff = Request.Form["IdGastoHHstaff"];
                var tiporecursostaff = Request.Form["Tiporecursostaff"];
                var messtaff = Request.Form["Messtaff"];
                var aniostaff = Request.Form["Aniostaff"];
                var subtotalstafflist = Request.Form["Subtotalstaff"];
                var hhtotalesstafflist = Request.Form["HHtotalesstaff"];
                var fechapagostaff = Request.Form["Fechapagostaff"];
                var reajustestafflist = Request.Form["Reajustestaff"];
                var montostafflist = Request.Form["Montostaff"];
                var observacionstaff = Request.Form["Observacionstaff"];
                var costounitariostafflist = Request.Form["CostoUnitarioStaff"];





                List<GastosHHViewModel> gastostaff = new List<GastosHHViewModel>();

                for (int i = 0; i < idgastohhstaff.Count; i++)
                {
                    var subtotalstaffStr = subtotalstafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(subtotalstaffStr))
                    {
                        subtotalstaffStr = "0";
                    }
                    else
                    {
                        subtotalstaffStr = subtotalstaffStr.Replace(".", "");
                    }

                    decimal subtotalstaff = decimal.Parse(subtotalstaffStr);


                    var reajustestaffStr = reajustestafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(reajustestaffStr))
                    {
                        reajustestaffStr = "0";
                    }
                    else
                    {
                        reajustestaffStr = reajustestaffStr.Replace(".", "");
                    }



                    decimal reajustestaff = decimal.Parse(reajustestaffStr);


                    var montostaffStr = montostafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(montostaffStr))
                    {
                        montostaffStr = "0";
                    }
                    else
                    {
                        montostaffStr = montostaffStr.Replace(".", "");
                    }



                    decimal montostaff = decimal.Parse(montostaffStr);


                    var hhtotalesstaffStr = hhtotalesstafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(hhtotalesstaffStr))
                    {
                        hhtotalesstaffStr = "0";
                    }
                    else
                    {
                        hhtotalesstaffStr = hhtotalesstaffStr.Replace(".", "");
                    }



                    decimal hhtotalesstaff = decimal.Parse(hhtotalesstaffStr);


                    var costounitariostaffStr = costounitariostafflist[i]?.ToString().Trim() ?? "";
                    if (string.IsNullOrEmpty(costounitariostaffStr))
                    {
                        costounitariostaffStr = "0";
                    }
                    else
                    {
                        costounitariostaffStr = costounitariostaffStr.Replace(".", "");
                    }

                    decimal costounitariostaff = decimal.Parse(costounitariostaffStr);



                    int idgastohhstaffRealParsed = string.IsNullOrWhiteSpace(idgastohhstaff[i])
                                               ? 0
                                               : int.Parse(idgastohhstaff[i]);



                    DateTime? fechapagostaffParsed;
                    if (string.IsNullOrWhiteSpace(fechapagostaff[i]))
                    {
                        fechapagostaffParsed = null;
                    }
                    else
                    {
                        fechapagostaffParsed = DateTime.Parse(fechapagostaff[i]);
                    }
                    var gastohhstaffViewModel = new GastosHHViewModel
                    {
                        IdGastoHH = idgastohhstaffRealParsed,
                        Tiporecurso = tiporecursostaff[i],
                        Mes = int.Parse(messtaff[i]),
                        Anio = int.Parse(aniostaff[i]),
                        Fechapago = fechapagostaffParsed,
                        Monto = montostaff,
                        HHtotales = hhtotalesstaff,
                        Observacion = observacionstaff[i],
                        Subtotal = subtotalstaff,
                        Reajuste = reajustestaff,
                        costonitario = costounitariostaff,
                    };

                    gastostaff.Add(gastohhstaffViewModel);


                }





                var gastohhstaff = ProcesarGastosHH(
                    Request.Form,
                    "Idhhstaff",
                    "recursohhstaff",
                    "montohhstaff",
                    "hhstaff",
                    "fechahhstaff",
                    "observacionhhstaff",
                    "estadohhstaff",
                    "EliminarhhStaff",
                    "costohhstaff"
                );


                await proyectoService.GestorFechaModificacionProyecto(idproyecto);
                await ejecucionService.GestorServiciosReales(idproyecto, servicios);
                await ejecucionService.GestorServiciosReales(idproyecto, servicioshonorarios);
                await ejecucionService.GestorGastosReales(idproyecto, gastos);
                await ejecucionService.GestorGastosHH(idproyecto, gastostaff);
                await ejecucionService.GestorGastosHH(idproyecto, gastosocioo);
                await ejecucionService.GestorGastosHH(idproyecto, gastohhsocio);
                await ejecucionService.GestorGastosHH(idproyecto, gastohhstaff);
                await proyectoService.AgregarGastoProyectoeJECUCION(idproyecto, verificargastos);
                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarserviciootro);
                await proyectoService.AgregarServicioProyectoeJECUCION(idproyecto, verificarserviciohonorario);
                TempData["SuccessMessage"] = "Los costos del proyecto se han registrado y actualizado correctamente.";
                return RedirectToAction("ForecastCostos", "EjecucionProyecto", new { id = idproyecto });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al registrar/editar costos del proyecto:{e.Message}");
                TempData["ErrorMessage"] = "Hubo un error al Registrar/Editar costos del proyecto.";
                return RedirectToAction("ForecastCostos", "EjecucionProyecto", new { id = idproyecto });
            }

        }

        private List<ServiciosRealesViewModel> ProcesarServicios(List<string> ids, List<string> proveedores, List<string> montos, List<string> fechas, List<string> idsReales, List<string> observaciones, List<string> estados, List<string> tipos, List<bool> eliminar)
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


        private List<GastosHHViewModel> ProcesarGastosHH(IFormCollection form, string prefixId, string prefixTipoRecurso, string prefixMonto, string prefixHH, string prefixFecha, string prefixObs, string prefixEstado, string prefixEliminar, string prefixCostos)
        {
            var ids = form[prefixId];
            var tipos = form[prefixTipoRecurso];
            var montos = form[prefixMonto];
            var hhs = form[prefixHH];
            var fechas = form[prefixFecha];
            var observaciones = form[prefixObs];
            var estados = form[prefixEstado];
            var eliminados = form[prefixEliminar];
            var costos = form[prefixCostos];

            var lista = new List<GastosHHViewModel>();

            for (int i = 0; i < fechas.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(fechas[i]))
                    continue;

                decimal monto = ParseDecimalSafe(montos[i]);
                decimal costo = ParseDecimalSafe(costos[i]);
                decimal hh = ParseDecimalSafe(hhs[i]);
                int id = string.IsNullOrWhiteSpace(ids[i]) ? 0 : int.Parse(ids[i]);
                DateTime? fecha = string.IsNullOrWhiteSpace(fechas[i]) ? null : DateTime.Parse(fechas[i]);

                var viewModel = new GastosHHViewModel
                {
                    IdGastoHH = id,
                    Tiporecurso = tipos[i],
                    Fechapago = fecha,
                    HHtotales = hh,
                    Observacion = observaciones[i],
                    Subtotal = monto,
                    Estado = int.Parse(estados[i]),
                    EsEliminado = eliminados[i] == "true",
                    costonitario = costo
                };

                lista.Add(viewModel);
            }

            return lista;
        }

        private decimal ParseDecimalSafe(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            value = value.Replace(".", "").Trim();
            return decimal.TryParse(value, out var result) ? result : 0;
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
            var proyecto = await proyectoService.ObtenerProyectos(id, idcliente, nombre, idtipoempresa, statusproyecto, numproyecto, idtipologia, unidadneg, idccosto, idusuario);
            ViewBag.Proyecto = proyecto;
            return View("flujocaja");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFlujo(int id)
        {
            var flujo = await reporteService.ObtenerFlujoCajaDetalle(id);
            return Ok(flujo);
        }



        /*Guardar Distribucion HH Masivos*/
        [HttpPost]
        public async Task<IActionResult> GuardarDistribucionHHMasiva(List<GastosHHViewModel> gastos)
        {
            try
            {
                await ejecucionService.GestorGastosHH(0, gastos);
                var modificacion = gastos
                    .Where(g => g.Fechapago != null)
                    .Select(g => new MoficacionProyectoViewModel
                    {
                        IdProyecto = int.Parse(g.idproyecto),
                        FechaPago = g.Fechapago
                    })
                    .Distinct()
                    .ToList();
                await proyectoService.GestorFechaModificacionProyectoMasivo(modificacion);
                TempData["SuccessMessageGastosHH"] = "Los pagos de HH socios/staff se han registrado correctamente.";
                return RedirectToAction("PagosDistribucionHH", new { estado = 0 });
            }

            catch (Exception ex)
            {

                Debug.WriteLine(ex.InnerException);
                TempData["ErrorMessageGastosHH"] = "Hubo un error al Registrar pagos de distribución hh de los socios y staff.";
                return RedirectToAction("PagosDistribucionHH", new { estado = 0 });
            }

        }

    }
}
