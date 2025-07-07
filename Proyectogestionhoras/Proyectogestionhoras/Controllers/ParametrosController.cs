using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Services;
using Proyectogestionhoras.Services.Interface;
using System.Diagnostics;

namespace Proyectogestionhoras.Controllers
{
    public class ParametrosController : Controller
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly BonoService bonoService;
        public ParametrosController(PROYECTO_CONTROL_HORASContext context,BonoService bonoService)
        {
            this.context = context;
            this.bonoService = bonoService;
        }

        public IActionResult TodoslosParametros()
        {

            return View("Parametros");
        }

        /*Cuentas*/
        public async Task<IActionResult> GestorCuentas() 
        {

            var cuentas = await ObtenerCuentas();
            var tipocuenta = await context.Tipocuenta.ToListAsync();
            ViewBag.Cuentas = cuentas;
            ViewBag.Tipocuentas = tipocuenta;
            return View();
        
        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarCuenta(Cuentum cuentas)
        {
            if (cuentas == null)
            {
                TempData["Errorcuenta"] = "No se pudo crear/actualizar la cuenta.";
                return RedirectToAction("GestorCuentas");
            }

            try
            {
                if (cuentas.Id == 0)
                {

                    context.Cuenta.Add(cuentas);
                    TempData["Successcuenta"] = "Cuenta creada Éxitosamente.";
                }
                else
                {

                    var cuentaExistente = await context.Cuenta.FindAsync(cuentas.Id);
                    if (cuentaExistente == null)
                    {
                        TempData["Errorcuenta"] = "Cuenta no Encontrada.";
                        return RedirectToAction("GestorDiaPago");
                    }

                    cuentaExistente.Cuenta = cuentas.Cuenta;
                    cuentaExistente.Idtipocuenta = cuentas.Idtipocuenta;
                    context.Cuenta.Update(cuentaExistente);
                    TempData["Successcuenta"] = "Cuenta actualizada correctamente.";
                }


                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                TempData["Errorcuenta"] = "Ocurrió un error al guardar la cuenta.";
                Debug.WriteLine($"Hubo un error al gaurdar la cuenta:{ex.InnerException}");
            }
           
            return RedirectToAction("GestorCuentas");
        }

        public async Task<List<Cuentum>> ObtenerCuentas()
        {
            var cuentas = await context.Cuenta.Include(c=>c.IdtipocuentaNavigation).OrderBy(c=>c.Idcuenta).ToListAsync();
            return cuentas;
        }

        /*Gastos*/
        public async Task<IActionResult> GestorGastos()
        {

            var gastos = await context.Gastos.ToListAsync();
            var cuentas = await context.Cuenta.OrderBy(c=>c.Idcuenta).ToListAsync();
            var gastosConCuentas = from gasto in gastos
                                   join cuenta in cuentas on gasto.Idcuenta equals cuenta.Id
                                   select new
                                   {
                                       gasto.Id,
                                       gasto.Nombre,
                                       CuentaNombre = cuenta.Cuenta, 
                                       gasto.Idcuenta
                                   };
            ViewBag.Cuentas = cuentas;
            ViewBag.Gastos = gastosConCuentas;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarGasto(Gasto gastos)
        {
            if (gastos == null)
            {
                TempData["ErrorGasto"] = "Error: El gasto es nulo.";
                return RedirectToAction("GestorGastos");
            }

            try
            {
                if (gastos.Id == 0)
                {
                    context.Gastos.Add(gastos);
                    TempData["SuccessGasto"] = "Gasto agregado correctamente.";
                }
                else
                {
                    var gastoexistente = await context.Gastos.FindAsync(gastos.Id);
                    if (gastoexistente == null)
                    {
                        TempData["ErrorGasto"] = "Error: Gasto no encontrado.";
                        return RedirectToAction("GestorGastos");
                    }

                    gastoexistente.Nombre = gastos.Nombre;
                    gastoexistente.Idcuenta = gastos.Idcuenta;
                    context.Gastos.Update(gastoexistente);

                    TempData["SuccessGasto"] = "Gasto actualizado correctamente.";
                }

                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["ErrorGasto"] = "Ocurrió un error al guardar el gasto.";
            }

            return RedirectToAction("GestorGastos");
        }

        /*Servicios*/
        public async Task<IActionResult> GestorServicios()
        {

            var servicios = await context.Servicios.ToListAsync();
            var cuentas = await context.Cuenta.OrderBy(c=>c.Idcuenta).ToListAsync();
            var servicioConCuentas = from servicio in servicios
                                     join cuenta in cuentas on servicio.Idcuenta equals cuenta.Id
                                   select new
                                   {
                                       servicio.Id,
                                       servicio.Nombre,
                                       CuentaNombre = cuenta.Cuenta,
                                       servicio.Idcuenta
                                   };
            ViewBag.Servicios = servicioConCuentas;
            ViewBag.Cuentas = cuentas;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> GuardarActualizarServicio(Servicio servicios)
        {
            if (servicios == null)
            {
                TempData["ErrorServicio"] = "Error: El servicio es nulo.";
                return RedirectToAction("GestorServicios");
            }

            try
            {
                if (servicios.Id == 0)
                {
                    context.Servicios.Add(servicios);
                    TempData["SuccessServicio"] = "Servicio agregado correctamente.";
                }
                else
                {
                    var servicioexistente = await context.Servicios.FindAsync(servicios.Id);
                    if (servicioexistente == null)
                    {
                        TempData["ErrorServicio"] = "Error: Servicio no encontrado.";
                        return RedirectToAction("GestorServicios");
                    }

                    servicioexistente.Nombre = servicios.Nombre;
                    servicioexistente.Idcuenta = servicios.Idcuenta;
                    context.Servicios.Update(servicioexistente);

                    TempData["SuccessServicio"] = "Servicio actualizado correctamente.";
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorServicio"] = "Ocurrió un error al guardar el servicio.";
            }

            return RedirectToAction("GestorServicios");
        }

        /*Actividades*/
        public IActionResult GestorActividades()
        {
            
            var actividades = context.Actividades.ToList();

            
            var actividadesAgrupadas = actividades
                .GroupBy(a => a.Nombre)
                .Select(g => new {
                    Nombre = g.Key,
                    Roles = string.Join(", ", g.Select(x => x.TipoAcatividad)),
                   
                    Controlhh = g.First().Controlhh,
                    
                    Ids = string.Join(",", g.Select(x => x.Id.ToString()))
                })
                .ToList();

            ViewBag.Actividades = actividadesAgrupadas;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GuardarActualizarActividades(string nombre, List<string> Roles, string controlhh, string id)
        {
            if (string.IsNullOrEmpty(nombre) || Roles == null || Roles.Count == 0)
            {
                return BadRequest("Nombre y al menos un rol son requeridos.");
            }

          
            var ids = string.IsNullOrEmpty(id) ? new List<int>() : id.Split(',').Select(int.Parse).ToList();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (ids.Any()) 
                {
                   
                    var actividadesExistentes = await context.Actividades
                        .Where(a => ids.Contains(a.Id))
                        .ToListAsync();

                    
                    foreach (var actividad in actividadesExistentes)
                    {
                        if (Roles.Contains(actividad.TipoAcatividad))
                        {
                            actividad.Nombre = nombre;
                            actividad.Controlhh = controlhh;
                            context.Update(actividad);
                            Roles.Remove(actividad.TipoAcatividad); 
                        }
                        else
                        {
                            context.Remove(actividad); 
                        }
                    }

                    
                    foreach (var nuevoRol in Roles)
                    {
                        context.Actividades.Add(new Actividade
                        {
                            Nombre = nombre,
                            TipoAcatividad = nuevoRol,
                            Controlhh = controlhh
                        });
                    }
                }
                else 
                {
                    
                    var existentes = await context.Actividades
                        .Where(a => a.Nombre == nombre && Roles.Contains(a.TipoAcatividad))
                        .AnyAsync();

                    if (existentes)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("Algunas combinaciones de nombre y rol ya existen.");
                    }

                    
                    foreach (var rol in Roles)
                    {
                        context.Actividades.Add(new Actividade
                        {
                            Nombre = nombre,
                            TipoAcatividad = rol,
                            Controlhh = controlhh
                        });
                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction("GestorActividades");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }


        /*SubActividades*/
        public async Task<IActionResult> GestorSubActividades()
        {
            var subactividades = await context.Subactividads.ToListAsync();
            var actividades = await context.Actividades.ToListAsync();
            var cuentas = await context.Cuenta.ToListAsync();
            var subactividadesconactividad = from subactividad in subactividades
                                     join actividad in actividades on subactividad.Idactividad equals actividad.Id
                                     join cuenta in cuentas on subactividad.Idcuenta equals cuenta.Id into cuentaJoin
                                     from cuenta in cuentaJoin.DefaultIfEmpty()
                                     select new
                                     {
                                         subactividad.Id,
                                         subactividad.Nombre,
                                         ActividadNombre = actividad.Nombre,
                                         subactividad.Idactividad,
                                         RolActividad = actividad.TipoAcatividad,
                                         subactividad.Idcuenta,
                                         Cuenta = cuenta != null ? cuenta.Cuenta : null
                                     };
            ViewBag.SubActividades = subactividadesconactividad;
            ViewBag.Cuentas = cuentas;
            ViewBag.Actividades = actividades;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarSubActividades(Subactividad subactividad)
        {
            if (subactividad == null)
            {
                return BadRequest("La SubActividad es nula.");
            }


            if (subactividad.Id == 0)
            {

                context.Subactividads.Add(subactividad);
            }
            else
            {

                var subactividadexistente = await context.Subactividads.FindAsync(subactividad.Id);
                if (subactividadexistente == null)
                {
                    return NotFound("Actividad no encontrada.");
                }

                subactividadexistente.Nombre = subactividad.Nombre;
                subactividadexistente.Idactividad = subactividad.Idactividad;
                subactividadexistente.Idcuenta = subactividad.Idcuenta;
                context.Subactividads.Update(subactividadexistente);
            }


            await context.SaveChangesAsync();
            return RedirectToAction("GestorSubActividades");
        }

        /*Bonos*/

        public async Task<IActionResult> GestorBonos(int anio)
        {
            var bonos = await bonoService.RecuperarDatosBonos(anio);
            var montoactual = ObtenerMontoTrimestreActual(anio);
            ViewBag.Bonos = bonos;
            ViewBag.Metaq = montoactual;
            return View();
        }


        public decimal? ObtenerMontoTrimestreActual(int anio)
        {
         
            int mesActual = DateTime.Now.Month;
            int trimestreActual = (mesActual + 2) / 3; 

           
                var registro = context.MetaFacturacionesqxes
                                     .FirstOrDefault(m => m.Anio == anio);

                if (registro != null)
                {
                    
                    switch (trimestreActual)
                    {
                        case 1: return registro.MontoQ1;
                        case 2: return registro.MontoQ2;
                        case 3: return registro.MontoQ3;
                        case 4: return registro.MontoQ4;
                        default: return null;
                    }
                }
                return null;
            
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarBonos(Bono bonos)
        {
            if (bonos == null)
            {
                return BadRequest("El Bono es nulo.");
            }

            
            var bonosExistentes = await context.Bonos
                .Where(b => b.Anio == bonos.Anio && b.Porcentaje == bonos.Porcentaje)
                .ToListAsync();

            if (!bonosExistentes.Any())
            {
                return NotFound("No se encontraron bonos que actualizar.");
            }

            
            foreach (var bono in bonosExistentes)
            {
                bono.Monto = bonos.Monto; 
            }
            context.Bonos.UpdateRange(bonosExistentes);
            await context.SaveChangesAsync();

            return RedirectToAction("GestorBonos", new { anio = DateTime.Now.Year });
        }

        /*Segmentos*/
        public async Task<IActionResult> GestorSegmentos()
        {
            var query = from un in context.CcostoUnegocios
                        join sc in context.SegmentoCcostos on un.Id equals sc.IdCcosto
                        join s in context.Segmentos on sc.IdSegmento equals s.Id
                        join c in context.Cuenta on s.IdCuenta equals c.Id
                        select new
                        {   
                            idsegmento = s.Id,
                            tiposegmento = s.TipoSegmento,
                            SegmentoNombre = s.Nombre,
                            UnidadCodigo = un.Codigo,
                            CuentaId = c.Idcuenta,
                            CuentaNombre = c.Cuenta,
                            idcuenta = s.IdCuenta,
                            Idunidad = un.Id,
                        };

            var segmentos = await query.ToListAsync();
            var cuentas = await context.Cuenta.OrderBy(c => c.Idcuenta).ToListAsync();
            var ccostos = await context.CcostoUnegocios.ToListAsync();
            ViewBag.Ccostos = ccostos;
            ViewBag.Cuentas = cuentas;
            ViewBag.Segmentos = segmentos;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> GuardarActualizarSegmentos(Segmento segmentos)
        {
            try
            {
                if (segmentos == null)
                {
                    return BadRequest("El segmento es nulo.");
                }

                var ccosto = await context.CcostoUnegocios.ToListAsync();

                if (segmentos.Id == 0)
                {
                    
                    context.Segmentos.Add(segmentos);
                    await context.SaveChangesAsync(); 

                   
                    foreach (var item in ccosto)
                    {
                        var nuevoSegmentoCcosto = new SegmentoCcosto
                        {
                            IdSegmento = segmentos.Id,
                            IdCcosto = item.Id 
                        };

                        context.SegmentoCcostos.Add(nuevoSegmentoCcosto);
                    }

                    await context.SaveChangesAsync();
                }
                else
                {
                    // Editar segmento existente
                    var segmentoExistente = await context.Segmentos.FindAsync(segmentos.Id);

                    if (segmentoExistente == null)
                    {
                        return NotFound("Segmento no encontrado.");
                    }

                    segmentoExistente.Nombre = segmentos.Nombre;
                    segmentoExistente.TipoSegmento = segmentos.TipoSegmento;
                    segmentoExistente.IdCuenta = segmentos.IdCuenta;

                    context.Segmentos.Update(segmentoExistente);
                    await context.SaveChangesAsync();

                    // Eliminar los registros anteriores (si se quiere actualizar la relación completamente)
                    var relacionesAnteriores = context.SegmentoCcostos
                        .Where(x => x.IdSegmento == segmentos.Id);

                    context.SegmentoCcostos.RemoveRange(relacionesAnteriores);

                    // Agregar nuevas relaciones
                    foreach (var item in ccosto)
                    {
                        var nuevoSegmentoCcosto = new SegmentoCcosto
                        {
                            IdSegmento = segmentos.Id,
                            IdCcosto = item.Id
                        };

                        context.SegmentoCcostos.Add(nuevoSegmentoCcosto);
                    }

                    await context.SaveChangesAsync();
                }

                return RedirectToAction("GestorSegmentos");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar el segmento: {ex.Message}");
                return View();
            }
        }


        /*Tipologias*/
        public async Task<IActionResult> GestorTipologias()
        {

            var tipologias = await context.Tipologia
                              .OrderByDescending(t => t.Id)
                              .ToListAsync();
            ViewBag.Tipologias = tipologias;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarTipologias(Tipologium tipologias)
        {
            if (tipologias == null)
            {
                return BadRequest("La tipología es nula.");
            }


            if (tipologias.Id == 0)
            {

                context.Tipologia.Add(tipologias);
            }
            else
            {

                var tipologiaexistente = await context.Tipologia.FindAsync(tipologias.Id);
                if (tipologiaexistente == null)
                {
                    return NotFound("tipología no encontrada.");
                }
                decimal monto = decimal.Parse(Request.Form["Monto"].ToString());

                tipologiaexistente.TipoTipologia = tipologias.TipoTipologia;
                tipologiaexistente.Monto = monto;
                tipologiaexistente.Desde = tipologias.Desde;
                tipologiaexistente.Hasta = tipologias.Hasta;
                tipologiaexistente.Hhsocios = tipologias.Hhsocios;
                tipologiaexistente.Hhstaff = tipologias.Hhstaff;
                tipologiaexistente.Consultora = tipologias.Consultora;
                tipologiaexistente.Consutlrob = tipologias.Consutlrob;
                tipologiaexistente.Consultorc = tipologias.Consultorc;
                context.Tipologia.Update(tipologiaexistente);
            }


            await context.SaveChangesAsync();
            return RedirectToAction("GestorTipologias");
        }

        /*Proveedores*/
        public async Task<IActionResult> GestorProveedores()
        {

            var proveedores = await context.Proveedores.OrderBy(p=>p.Nombre).ToListAsync();
            ViewBag.Proveedores = proveedores;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarProveedor(Proveedore proveedor, [FromForm] List<string> Tipo)
        {
            if (proveedor == null)
            {
                TempData["ErrorProveedor"] = "Error: El proveedor es nulo.";
                return RedirectToAction("GestorProveedores", "Parametros");
            }

            if (string.IsNullOrEmpty(proveedor.Rut) || string.IsNullOrEmpty(proveedor.Nombre) || string.IsNullOrEmpty(proveedor.Funcion))
            {
                TempData["ErrorProveedor"] = "Todos los campos requeridos deben estar llenos.";
                return RedirectToAction("GestorProveedores", "Parametros");
            }

            try
            {
                proveedor.Tipo = string.Join(",", Tipo);

                if (proveedor.Id == 0)
                {
                    await context.Proveedores.AddAsync(proveedor);
                    TempData["SuccessProveedor"] = "Proveedor agregado correctamente.";
                }
                else
                {
                    var proveedorExistente = await context.Proveedores.FindAsync(proveedor.Id);
                    if (proveedorExistente == null)
                    {
                        TempData["ErrorProveedor"] = "Error: Proveedor no encontrado.";
                        return RedirectToAction("GestorProveedores", "Parametros");
                    }

                    proveedorExistente.Nombre = proveedor.Nombre;
                    proveedorExistente.Funcion = proveedor.Funcion;
                    proveedorExistente.Tipo = proveedor.Tipo;

                    context.Proveedores.Update(proveedorExistente);
                    TempData["SuccessProveedor"] = "Proveedor actualizado correctamente.";
                }

                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["ErrorProveedor"] = "Ocurrió un error al guardar el proveedor.";
            }

            return RedirectToAction("GestorProveedores", "Parametros");
        }



        /*Dia Pago*/
        public async Task<IActionResult> GestorDiaPago()
        {

            var diapago = await context.Diapagos.FirstOrDefaultAsync();
            return View(diapago);

        }

        [HttpPost]
        public async Task<IActionResult> GuardarActualizarDiaPago(Diapago dias)
        {
            if (dias == null)
            {
                TempData["Errordia"] = "No se pudo actualizar el día de pago.";
                return RedirectToAction("GestorDiaPago");
            }
            if (dias.Dia < 1 || dias.Dia > 31)
            {
                TempData["Errordia"] = "El día de pago debe estar entre 1 y 31.";
                return RedirectToAction("GestorDiaPago");
            }

            try
            {
                if (dias.Id == 0)
                {
                    context.Diapagos.Add(dias);
                    TempData["Successdia"] = "Día de pago asignado correctamente.";
                }
                else
                {
                    var diaExistente = await context.Diapagos.FindAsync(dias.Id);
                    if (diaExistente == null)
                    {
                        TempData["Errordia"] = "Día no encontrado.";
                        return RedirectToAction("GestorDiaPago");
                    }

                    diaExistente.Dia = dias.Dia;
                    context.Diapagos.Update(diaExistente);
                    TempData["Successdia"] = "Día de pago actualizado correctamente.";
                }

                await context.SaveChangesAsync();
            }
            catch
            {
                TempData["Errordia"] = "Ocurrió un error al guardar el día de pago.";
            }

            return RedirectToAction("GestorDiaPago");
        }

    }
}
