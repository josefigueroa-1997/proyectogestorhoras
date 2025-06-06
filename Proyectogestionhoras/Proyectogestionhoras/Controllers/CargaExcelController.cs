using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services;
namespace Proyectogestionhoras.Controllers
{


    public class CargaExcelController : Controller
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly ExcelService excelService;

        public CargaExcelController(PROYECTO_CONTROL_HORASContext context,ExcelService excelService) {
            this.context = context;
            this.excelService = excelService;
        }
      /*Egresos*/
        public IActionResult VistaPreviaEgresos()
        {
            if (TempData["ExcelData"] != null)
            {
                var json = TempData["ExcelData"].ToString();
                var datos = System.Text.Json.JsonSerializer.Deserialize<List<EgresosExcelDTO>>(json);
                return View(datos);
            }

            return RedirectToAction("Index","Home");
            
        }

        [HttpPost]
         public IActionResult CargarExcel(IFormFile archivo)
         {
             List<EgresosExcelDTO> listaServicios = new List<EgresosExcelDTO>();

             if (archivo != null && archivo.Length > 0)
             {
                 using (var stream = archivo.OpenReadStream())
                 using (var package = new ExcelPackage(stream))
                 {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet hoja = package.Workbook.Worksheets[0];
                     int totalFilas = hoja.Dimension.Rows;
                    int totalColumnas = hoja.Dimension.Columns;
                    if (totalColumnas != 9)
                    {

                        TempData["ExcelErrorEgreso"] = "El Documento Excel que usted ha subido no tiene un formato compatible.Por favor, ingrese uno correcto.";
                        return RedirectToAction("Index", "Home");
                    }
                    var encabezadoColumna2 = hoja.Cells[1, 2].Text.Trim();
                    if (encabezadoColumna2.Equals("NumDocumento", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["ExcelErrorEgreso"] = "El Documento Excel que usted ha subido no tiene un formato compatible.Por favor, ingrese uno correcto.";
                        return RedirectToAction("Index", "Home");
                    }
                    for (int fila = 2; fila <= totalFilas; fila++) 
                     {
                         var proyecto = hoja.Cells[fila, 1].Text;
                         var servicio = hoja.Cells[fila, 2].Text;
                         var proveedor = hoja.Cells[fila, 3].Text;
                         var montoTexto = hoja.Cells[fila, 4].Text;
                         var fechaTexto = hoja.Cells[fila, 5].Text;
                         var estado = hoja.Cells[fila, 6].Text;
                         var estadoventa = hoja.Cells[fila, 7].Text;
                         var glosa = hoja.Cells[fila, 8].Text;
                         var tipo = hoja.Cells[fila, 9].Text;

                         if (string.IsNullOrWhiteSpace(proyecto) &&
                             string.IsNullOrWhiteSpace(servicio) &&
                             string.IsNullOrWhiteSpace(proveedor) &&
                             string.IsNullOrWhiteSpace(montoTexto) &&
                             string.IsNullOrWhiteSpace(fechaTexto) &&
                             string.IsNullOrWhiteSpace(estado) &&
                             string.IsNullOrWhiteSpace(estadoventa) &&
                             string.IsNullOrWhiteSpace(glosa) &&
                             string.IsNullOrWhiteSpace(tipo) )
                             continue;

                         if (decimal.TryParse(montoTexto, out decimal monto) &&
                             DateTime.TryParse(fechaTexto, out DateTime fecha))
                         {
                             listaServicios.Add(new EgresosExcelDTO
                             {
                                 numproyecto = proyecto,
                                 egreso = servicio,
                                 proveedor = proveedor,
                                 Monto = monto,
                                 Fecha = fecha,
                                 Estado = estado,
                                 EstadoVenta = estadoventa,
                                 Glosa = glosa,
                                 Tipo = tipo,
                             });
                         }
                     }
                 }
             }


            TempData["ExcelData"] = System.Text.Json.JsonSerializer.Serialize(listaServicios);

            return RedirectToAction("VistaPreviaEgresos");
         }



        [HttpPost]
        public async Task<IActionResult> GuardarEgresosMasivos()
        {
            try
            {
                
                var form = await Request.ReadFormAsync();

              
                var egresos = new List<EgresosExcelViewModel>();

             
                var numProyectos = form["numproyecto"].ToList();
                var egresosNombres = form["egreso"].ToList();
                var proveedores = form["proveedor"].ToList();
                var montos = form["Monto"].ToList();
                var fechas = form["Fecha"].ToList();
                var estados = form["Estado"].ToList();
                var estadosVenta = form["EstadoVenta"].ToList();
                var glosas = form["Glosa"].ToList();
                var tipos = form["Tipo"].ToList();
                var idsProyecto = form["ProyectoId"].ToList();
                var idsEgreso = form["IdEgreso"].ToList();
                var idsProveedor = form["IdsProveedor"].ToList();
                var idsSegmentos = form["idsegmento"].ToList();

                
                
                if (new[] { numProyectos.Count, egresosNombres.Count, proveedores.Count,
                   montos.Count, fechas.Count, estados.Count, estadosVenta.Count,
                   glosas.Count, tipos.Count, idsProyecto.Count, idsEgreso.Count,
                   idsProveedor.Count }.Distinct().Count() > 1)
                {
                    return BadRequest("Los datos recibidos no son consistentes");
                }

              
                for (int i = 0; i < numProyectos.Count; i++)
                {
                 
                    decimal monto;
                    if (!decimal.TryParse(montos[i].Replace("$", "").Replace(",", ""), out monto))
                    {
                        return BadRequest($"Formato de monto inválido en la fila {i + 1}");
                    }

                    DateTime fecha;
                    if (!DateTime.TryParse(fechas[i], out fecha))
                    {
                        return BadRequest($"Formato de fecha inválido en la fila {i + 1}");
                    }

                    int idProyecto;
                    if (!int.TryParse(idsProyecto[i], out idProyecto))
                    {
                        return BadRequest($"ID de proyecto inválido en la fila {i + 1}");
                    }

                    int idEgreso;
                    if (!int.TryParse(idsEgreso[i], out idEgreso))
                    {
                        return BadRequest($"ID de egreso inválido en la fila {i + 1}");
                    }

                    int idProveedor;
                    if (!int.TryParse(idsProveedor[i], out idProveedor))
                    {
                        return BadRequest($"ID de proveedor inválido en la fila {i + 1}");
                    }

                  
                    var egreso = new EgresosExcelViewModel
                    {
                        Idpeoyecto = idProyecto,
                        Idegreso = idEgreso,
                        Idproveedor = idProveedor,
                        Monto = monto,
                        Fecha = fecha,
                        Observacion = glosas[i],
                        Estado = estados[i],
                        Venta = estadosVenta[i],
                        Tiposervicio = tipos[i]
                    };

                    egresos.Add(egreso);
                }

               
                await excelService.IngresarEgresosMasivosExcel(egresos);

                List<ServicioViewModel> verificarserviciohonorario = new List<ServicioViewModel>();

                for (int i = 0; i < numProyectos.Count; i++)
                {
                    if (tipos[i] == "Gastos") continue; 

                    var servicioViewModel = new ServicioViewModel
                    {
                        Idservicios = int.Parse(idsEgreso[i]),
                        IdSegmento = int.Parse(idsSegmentos[i]),
                        IdsProyecto = new List<int> { int.Parse(idsProyecto[i]) }
                    };

                    verificarserviciohonorario.Add(servicioViewModel);
                }
                await excelService.AgregarServicioProyectonuevos(verificarserviciohonorario);

                List<GastoViewModel> gastosaingresar = new List<GastoViewModel>();
                for (int i = 0; i < numProyectos.Count; i++)
                {
                    if (tipos[i] != "Gastos") continue;

                    var gastoViewModel = new GastoViewModel
                    {
                        Idgastos = int.Parse(idsEgreso[i]),
                        IdSegmento = int.Parse(idsSegmentos[i]),
                        IdProyecto = new List<int> { int.Parse(idsProyecto[i]) }
                    };

                    gastosaingresar.Add(gastoViewModel);
                }

                await excelService.AgregarGastosProyectonuevos(gastosaingresar);

                return Ok(new { success = true, message = "Egresos guardados correctamente" });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message;
                Debug.WriteLine($"DbUpdateException: {ex.Message}");
                Debug.WriteLine($"Inner: {inner}");
                return StatusCode(500, new { success = false, message = $"Error al guardar los egresos" });
            }
        }

        /*Ingresos*/
        [HttpPost]
        public IActionResult CargarIngresosExcel(IFormFile archivoingreso)
        {
            List<IngresosExcelDTO> listaingresos = new List<IngresosExcelDTO>();

            if (archivoingreso != null && archivoingreso.Length > 0)
            {
                using (var stream = archivoingreso.OpenReadStream())
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet hoja = package.Workbook.Worksheets[0];
                    int totalFilas = hoja.Dimension.Rows;
                    int totalColumnas = hoja.Dimension.Columns;
                    if (totalColumnas != 9)
                    {

                        TempData["ExcelErrorIngreso"] = "El Documento Excel que usted ha subido no tiene un formato compatible.Por favor, ingrese uno correcto.";
                        return RedirectToAction("Index", "Home");
                    }
                    var encabezadoColumna2 = hoja.Cells[1, 2].Text.Trim();
                    if (!encabezadoColumna2.Equals("NumDocumento", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["ExcelErrorIngreso"] = "El Documento Excel que usted ha subido no tiene un formato compatible.Por favor, ingrese uno correcto.";
                        return RedirectToAction("Index","Home");
                    }
                    for (int fila = 2; fila <= totalFilas; fila++)
                    {
                        var proyecto = hoja.Cells[fila, 1].Text;
                        var numdocumento = hoja.Cells[fila, 2].Text;
                        var fechaemisiontexto = hoja.Cells[fila, 3].Text;
                        var fechapagotexto = hoja.Cells[fila, 4].Text;
                        var montotexto = hoja.Cells[fila, 5].Text;
                        var ivatexto = hoja.Cells[fila, 6].Text;
                        var estado = hoja.Cells[fila, 7].Text;
                        var estadoventa = hoja.Cells[fila, 8].Text;
                        var glosa = hoja.Cells[fila, 9].Text;

                        if (string.IsNullOrWhiteSpace(proyecto) &&
                            string.IsNullOrWhiteSpace(numdocumento) &&
                            string.IsNullOrWhiteSpace(fechaemisiontexto) &&
                            string.IsNullOrWhiteSpace(fechapagotexto) &&
                            string.IsNullOrWhiteSpace(montotexto) &&
                            string.IsNullOrWhiteSpace(ivatexto) &&
                            string.IsNullOrWhiteSpace(estado) &&
                            string.IsNullOrWhiteSpace(estadoventa) &&
                            string.IsNullOrWhiteSpace(glosa))
                            continue;

                        if (decimal.TryParse(montotexto, out decimal monto) &&
                            decimal.TryParse(ivatexto, out decimal iva) &&
                            DateTime.TryParse(fechaemisiontexto, out DateTime fechaemision) &&
                            DateTime.TryParse(fechapagotexto, out DateTime fechapago))
                        {
                            listaingresos.Add(new IngresosExcelDTO
                            {
                                numproyecto = proyecto,
                                numdocumento = numdocumento,
                                fechaemision = fechaemision,
                                Fechapago = fechapago,
                                Monto = monto,
                                iva = iva,
                                Estado = estado,
                                EstadoVenta = estadoventa,
                                Glosa = glosa,
                                
                            });
                        }
                    }
                }
            }


            TempData["ExcelingresosData"] = System.Text.Json.JsonSerializer.Serialize(listaingresos);

            return RedirectToAction("VistaPreviaIngresos");
        }

        public IActionResult VistaPreviaIngresos()
        {
            if (TempData["ExcelingresosData"] != null)
            {
                var json = TempData["ExcelingresosData"].ToString();
                var datos = System.Text.Json.JsonSerializer.Deserialize<List<IngresosExcelDTO>>(json);
                return View(datos);
            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> GuardarIngresosMasivos()
        {
            try
            {

                var form = await Request.ReadFormAsync();


                var ingresos = new List<IngresosExcelViewModel>();


                var numProyectos = form["numproyecto"].ToList();
                var numdocumentos = form["numdocumento"].ToList();
                var fechaemisiones = form["fechaemision"].ToList();
                var montos = form["Monto"].ToList();
                var ivas = form["iva"].ToList();
                var Fechapagos = form["Fechapago"].ToList();
                var estados = form["Estado"].ToList();
                var estadosVenta = form["EstadoVenta"].ToList();
                var glosas = form["Glosa"].ToList();
                var idsProyecto = form["ProyectoId"].ToList();
              




                if (new[] { numProyectos.Count, numdocumentos.Count, fechaemisiones.Count,
                   montos.Count, ivas.Count, estados.Count, estadosVenta.Count,
                   glosas.Count, Fechapagos.Count, idsProyecto.Count}.Distinct().Count() > 1)
                {
                    return BadRequest("Los datos recibidos no son consistentes");
                }


                for (int i = 0; i < numProyectos.Count; i++)
                {

                    decimal monto;
                    if (!decimal.TryParse(montos[i].Replace("$", "").Replace(",", ""), out monto))
                    {
                        return BadRequest($"Formato de monto inválido en la fila {i + 1}");
                    }
                    decimal iva;
                    if (!decimal.TryParse(ivas[i].Replace("$", "").Replace(",", ""), out iva))
                    {
                        return BadRequest($"Formato de monto inválido en la fila {i + 1}");
                    }

                    DateTime fechaemision;
                    if (!DateTime.TryParse(fechaemisiones[i], out fechaemision))
                    {
                        return BadRequest($"Formato de fecha inválido en la fila {i + 1}");
                    }
                    DateTime fechapago;
                    if (!DateTime.TryParse(Fechapagos[i], out fechapago))
                    {
                        return BadRequest($"Formato de fecha inválido en la fila {i + 1}");
                    }

                    int idProyecto;
                    if (!int.TryParse(idsProyecto[i], out idProyecto))
                    {
                        return BadRequest($"ID de proyecto inválido en la fila {i + 1}");
                    }

                    


                    var ingreso = new IngresosExcelViewModel
                    {
                        Idproyecto = idProyecto,
                        numdocumento = numdocumentos[i],
                        Fechaemision = fechaemision,
                        Monto = monto,
                        iva = iva,
                        Fechapago = fechapago,
                        Observacion = glosas[i],
                        Estado = estados[i],
                        Venta = estadosVenta[i],
                        
                    };

                    ingresos.Add(ingreso);
                }


                await excelService.RegistrarIngresosMasivosExcel(ingresos);



                return Ok(new { success = true, message = "Ingresos guardados correctamente" });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message;
                Debug.WriteLine($"DbUpdateException: {ex.Message}");
                Debug.WriteLine($"Inner: {inner}");
                return StatusCode(500, new { success = false, message = $"Error al guardar los ingresos" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerIdProyecto(string numproyecto)
        {
            var resultado = await context.Proyectos.Where(p => p.NumProyecto == numproyecto).Select(p => p.Id).FirstOrDefaultAsync();
            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> obtenerSegmento(int idproyecto, int idcuenta, string tipo)
        {
            var ccostounegocio = await context.Proyectos.Where(p => p.Id == idproyecto).Select(p => p.IdCcostoUnegocio).FirstOrDefaultAsync();
            ServiciosDTO resultado = null;
            tipo = tipo?.Trim();
            if (tipo != "Gastos")
            {
                resultado = await (from cu in context.CcostoUnegocios
                                   join sc in context.SegmentoCcostos on cu.Id equals sc.IdCcosto
                                   join s in context.Segmentos on sc.IdSegmento equals s.Id
                                   where s.TipoSegmento == "Servicios" && s.IdCuenta == idcuenta && cu.Id == ccostounegocio
                                   select new ServiciosDTO
                                   {
                                       IDSEGMENTO = s.Id,
                                       NOMBRE = s.Nombre
                                   }).FirstOrDefaultAsync();
            }
            else
            {
                resultado = await (from cu in context.CcostoUnegocios
                                   join sc in context.SegmentoCcostos on cu.Id equals sc.IdCcosto
                                   join s in context.Segmentos on sc.IdSegmento equals s.Id
                                   where s.TipoSegmento == "Gastos" && s.IdCuenta == idcuenta && cu.Id == ccostounegocio
                                   select new ServiciosDTO
                                   {
                                       IDSEGMENTO = s.Id,
                                       NOMBRE = s.Nombre
                                   }).FirstOrDefaultAsync();
            }
            return Json(resultado);
        }

        private string LimpiarTexto(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Quitar espacios
            input = input.Trim().ToLowerInvariant();

            // Quitar acentos
            string normalized = input.Normalize(NormalizationForm.FormD);
            string sinAcentos = new string(normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Quitar símbolos raros (deja solo letras y números)
            sinAcentos = Regex.Replace(sinAcentos, @"[^a-z0-9]", "");

            return sinAcentos;
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerIdProveedor(string nombreproveedor)
        {
            nombreproveedor = nombreproveedor?.Trim();
            var resultado = await context.Proveedores.Where(p=>p.Nombre==nombreproveedor).Select(p=> p.Id).FirstOrDefaultAsync();
            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerIdEgreso(string nombreegreso)
        {
            int resultado = 0;
            nombreegreso = nombreegreso?.Trim();

         
            var servicioId = await context.Servicios
                .Where(p => p.Nombre == nombreegreso)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            if (servicioId != 0)
            {
                resultado = servicioId;
            }
            else
            {
                
                var gastoId = await context.Gastos
                    .Where(p => p.Nombre == nombreegreso)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                resultado = gastoId;
            }

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosCuenta(int idegreso, string tipo)
        {
            ServiciosProyectoDTO resultado = null;
            tipo = tipo?.Trim();
            if (tipo != "Gastos")
            {
                // Buscar en Servicios
                resultado = await (from s in context.Servicios
                                   join c in context.Cuenta on s.Idcuenta equals c.Id
                                   where s.Id == idegreso
                                   select new ServiciosProyectoDTO
                                   {
                                       Id = c.Id,
                                       IDCUENTA = c.Idcuenta,
                                       CUENTA = c.Cuenta
                                   }).FirstOrDefaultAsync();

               
            }
            else
            {
                // Si tipo es "Gastos", buscar solo en Gastos
                resultado = await (from g in context.Gastos
                                   join c in context.Cuenta on g.Idcuenta equals c.Id
                                   where g.Id == idegreso
                                   select new ServiciosProyectoDTO
                                   {
                                       Id = c.Id,
                                       IDCUENTA = c.Idcuenta,
                                       CUENTA = c.Cuenta
                                   }).FirstOrDefaultAsync();
            }

            return Json(resultado);
        }




    }
}
