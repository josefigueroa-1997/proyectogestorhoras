using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;
using System.Diagnostics;

namespace Proyectogestionhoras.Services
{
    public class EjecucionService : IEjecucion
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        public EjecucionService(PROYECTO_CONTROL_HORASContext context)
        {
            this.context = context;
        }

        public async Task GestorIngresos(int idproyecto, List<IngresoViewModel> ingresos)
        {
            try
            {
                if (ingresos == null || !ingresos.Any())
                {
                    return;
                }

                foreach (var ingreso in ingresos)
                {

                    var ingresoExistente = await context.Ingresosreales
                        .FirstOrDefaultAsync(i => i.Id == ingreso.Id);

                    if (ingresoExistente != null)
                    {

                        ingresoExistente.Numdocumento = ingreso.Numdocumento;
                        ingresoExistente.FechaEmision = ingreso.FechaEmision;
                        ingresoExistente.FechaVencimiento = ingreso.FechaVencimiento;
                        ingresoExistente.Montous = ingreso.Montous;
                        ingresoExistente.Tc = ingreso.Tc;
                        ingresoExistente.Montoclp = ingreso.Montoclp;
                        ingresoExistente.Iva = ingreso.Iva;
                        ingresoExistente.Estado = ingreso.Estado;
                        ingresoExistente.Idcuenta = ingreso.Idcuenta;
                        ingresoExistente.FechaPago = ingreso.FechaPago;
                    }
                    else
                    {

                        var nuevoIngreso = new Ingresosreale
                        {
                            Idproyecto = idproyecto,
                            Cuota = ingreso.Cuota + 1,
                            Numdocumento = ingreso.Numdocumento,
                            FechaEmision = ingreso.FechaEmision,
                            FechaVencimiento = ingreso.FechaVencimiento,
                            Montous = ingreso.Montous,
                            Tc = ingreso.Tc,
                            Montoclp = ingreso.Montoclp,
                            Iva = ingreso.Iva,
                            Estado = ingreso.Estado,
                            Idcuenta = ingreso.Idcuenta,
                            FechaPago = ingreso.FechaPago
                        };
                        await context.AddRangeAsync(nuevoIngreso);
                    }
                }


                await context.SaveChangesAsync();
            }

            catch(Exception e)
            {
                Debug.WriteLine($"Hubo un error al agregar o actualizar un ingreso:{e.Message}");
            }
           
        }

        public async Task GestorServiciosReales(int idproyecto, List<ServiciosRealesViewModel> servicios)
        {
            try
            {
                if (servicios == null || !servicios.Any())
                {
                    return;
                }

                foreach (var servicio in servicios)
                {
                    if (servicio.IdServicioReal > 0)
                    {
                        
                        var servicioexistente = await context.Serviciosejecucions
                                                             .FirstOrDefaultAsync(s => s.Id == servicio.IdServicioReal);
                        if (servicioexistente != null)
                        {
                            servicioexistente.Idservicio = servicio.Idservicio;
                            servicioexistente.Idproveedor = servicio.Idproveedor;
                            servicioexistente.Fecha = servicio.Fecha;
                            servicioexistente.Monto = servicio.Monto;
                        }
                    }
                    else
                    {
                        
                        var nuevoservicio = new Serviciosejecucion
                        {
                            Idproyecto = idproyecto,
                            Idservicio = servicio.Idservicio,
                            Idproveedor = servicio.Idproveedor,
                            Fecha = servicio.Fecha,
                            Monto = servicio.Monto,
                        };
                        await context.AddAsync(nuevoservicio);
                    }
                }

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al registrar/actualizar un servicio de ejecución: {e.Message}");
                if (e.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
            }
        }

        public async Task GestorGastosReales(int idproyecto,List<GastosRealesViewModel> gastos)
        {
            try
            {
                if (gastos == null || !gastos.Any())
                {
                    return;
                }
                foreach(var gasto in gastos)
                {
                    if (gasto.IdGastoReal > 0)
                    {
                        var gastoexistente = await context.Gastosejecucions.FirstOrDefaultAsync(g => g.Id == gasto.IdGastoReal);
                        if (gastoexistente != null)
                        {
                            gastoexistente.Idgasto = gasto.Idgasto;
                            gastoexistente.Idproveedor = gasto.Idproveedor;
                            gastoexistente.Segmento = gasto.Segmento;
                            gastoexistente.Monto = gasto.Monto;
                            gastoexistente.Fecha = gasto.Fecha;
                        }
                    }
                    else
                    {
                        var nuevogasto = new Gastosejecucion
                        {
                            Idproyecto = idproyecto,
                            Idgasto = gasto.Idgasto,
                            Idproveedor = gasto.Idproveedor,
                            Segmento = gasto.Segmento,
                            Monto = gasto.Monto,
                            Fecha = gasto.Fecha,
                        };
                        await context.AddRangeAsync(nuevogasto);


                    }
                }
                await context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error al registrar/actualizar un gastos de ejecución: {e.Message}");
                if (e.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
            }


        }
        public async Task<List<GastosRealesDTO>> ObtenerGastosReales(int? idproyecto)
        {
            try
            {
                var resultado = await (from p in context.Proyectos
                                       join g in context.Gastosejecucions on p.Id equals g.Idproyecto
                                       join s in context.Segmentos on g.Segmento equals s.Id
                                       join c in context.Cuenta on s.IdCuenta equals c.Id
                                       where p.Id == idproyecto
                                       select new GastosRealesDTO
                                       {
                                           IdGastosReal = g.Id,
                                           IdGasto = g.Idgasto,
                                           IdProveedor = g.Idproveedor,
                                           IdSegmento = s.Id,
                                           NombreSegmento = s.Nombre,
                                           Cuenta = c.Cuenta,
                                           IdCuenta = c.Idcuenta,
                                           Monto = g.Monto,
                                           Fecha = g.Fecha
                                       }).ToListAsync();

                return resultado;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error al obtener los gastos de ejecución: {e.Message}");
                return new List<GastosRealesDTO>();
            }
           
        }
    }
}
