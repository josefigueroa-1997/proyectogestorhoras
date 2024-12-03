using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Models.ViewModel;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace Proyectogestionhoras.Services
{
    public class EjecucionService : IEjecucion
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly Conexion conexion;
        private readonly ILogger<EjecucionService> logger;
        
        public EjecucionService(PROYECTO_CONTROL_HORASContext context, Conexion conexion,ILogger<EjecucionService> logger)
        {
            this.context = context;
            this.conexion = conexion;
            this.logger = logger;
         
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
                    if (ingreso.IdIngresoreal > 0)
                    {
                        var ingresoExistente = await context.Ingresosreales
                       .FirstOrDefaultAsync(i => i.Id == ingreso.IdIngresoreal);

                        if (ingresoExistente != null)
                        {

                            ingresoExistente.Numdocumento = ingreso.Numdocumento;
                            ingresoExistente.FechaEmision = ingreso.FechaEmision;
                            ingresoExistente.Montous = ingreso.Montous;
                            ingresoExistente.Tc = ingreso.Tc;
                            ingresoExistente.Montoclp = ingreso.Montoclp;
                            ingresoExistente.Iva = ingreso.Iva;
                            ingresoExistente.Estado = ingreso.Estado;
                            ingresoExistente.Idcuenta = ingreso.Idcuenta;
                            ingresoExistente.FechaPago = ingreso.FechaPago;
                            ingresoExistente.Observacion = ingreso.Observacion;
                        }
                    }
                   
                    else
                    {

                        var nuevoIngreso = new Ingresosreale
                        {
                            Idproyecto = idproyecto,
                            
                            Numdocumento = ingreso.Numdocumento,
                            FechaEmision = ingreso.FechaEmision,
                            Montous = ingreso.Montous,
                            Tc = ingreso.Tc,
                            Montoclp = ingreso.Montoclp,
                            Iva = ingreso.Iva,
                            Estado = ingreso.Estado,
                            Idcuenta = ingreso.Idcuenta,
                            FechaPago = ingreso.FechaPago,
                            Observacion = ingreso.Observacion
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

                var idsServicios = servicios.Where(s => s.IdServicioReal > 0)
                                            .Select(s => s.IdServicioReal)
                                            .ToList();

                var serviciosExistentes = await context.Serviciosejecucions
                                                      .Where(s => idsServicios.Contains(s.Id))
                                                      .ToDictionaryAsync(s => s.Id);

                var serviciosNuevos = servicios.Where(s => s.IdServicioReal <= 0).ToList();

                foreach (var servicio in servicios)
                {
                    if (servicio.IdServicioReal > 0 && serviciosExistentes.TryGetValue(servicio.IdServicioReal, out var servicioExistente))
                    {
                        servicioExistente.Idservicio = servicio.Idservicio;
                        servicioExistente.Idproveedor = servicio.Idproveedor;
                        servicioExistente.Fecha = servicio.Fecha;
                        servicioExistente.Monto = servicio.Monto;
                        servicioExistente.Observacion = servicio.Observacion;
                        servicioExistente.Estado = servicio.Estado;
                    }
                }

                var nuevosServicios = serviciosNuevos.Select(servicio => new Serviciosejecucion
                {
                    Idproyecto = idproyecto,
                    Idservicio = servicio.Idservicio,
                    Idproveedor = servicio.Idproveedor,
                    Fecha = servicio.Fecha,
                    Monto = servicio.Monto,
                    Observacion = servicio.Observacion,
                    Estado = servicio.Estado,
                }).ToList();

                await context.Serviciosejecucions.AddRangeAsync(nuevosServicios);
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

        public async Task GestorGastosReales(int idproyecto, List<GastosRealesViewModel> gastos)
        {
            try
            {
                if (gastos == null || !gastos.Any())
                {
                    return;
                }

               
                var idsGastos = gastos.Where(g => g.IdGastoReal > 0)
                                      .Select(g => g.IdGastoReal)
                                      .ToList();

          
                var gastosExistentes = await context.Gastosejecucions
                                                    .Where(g => idsGastos.Contains(g.Id))
                                                    .ToDictionaryAsync(g => g.Id);

              
                var nuevosGastos = gastos.Where(g => g.IdGastoReal <= 0).ToList();

               
                foreach (var gasto in gastos.Where(g => g.IdGastoReal > 0))
                {
                    if (gastosExistentes.TryGetValue(gasto.IdGastoReal, out var gastoExistente))
                    {
                        gastoExistente.Idgasto = gasto.Idgasto;
                        gastoExistente.Idproveedor = gasto.Idproveedor;
                        gastoExistente.Segmento = gasto.Segmento;
                        gastoExistente.Monto = gasto.Monto;
                        gastoExistente.Fecha = gasto.Fecha;
                        gastoExistente.Observacion = gasto.Observacion;
                        gastoExistente.Estado = gasto.Estado;
                    }
                }

                
                var gastosParaInsertar = nuevosGastos.Select(gasto => new Gastosejecucion
                {
                    Idproyecto = idproyecto,
                    Idgasto = gasto.Idgasto,
                    Idproveedor = gasto.Idproveedor,
                    Segmento = gasto.Segmento,
                    Monto = gasto.Monto,
                    Fecha = gasto.Fecha,
                    Observacion = gasto.Observacion,
                    Estado = gasto.Estado,
                }).ToList();

                await context.Gastosejecucions.AddRangeAsync(gastosParaInsertar);

           
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al registrar/actualizar un gasto de ejecución: {e.Message}");
                if (e.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
            }
        }

        public async Task GestorGastosHH(int idproyecto, List<GastosHHViewModel> gastosHH)
        {
            try
            {

                if (gastosHH == null || !gastosHH.Any())
                {
                    return;
                }

                foreach(var gasto in gastosHH)
                {
                    
                    gasto.Reajuste = gasto.Reajuste ?? 0;

                    
                    gasto.Monto = gasto.Subtotal + gasto.Reajuste;
                    var gastohhexistente = await context.Gastoshhhejecucions.FirstOrDefaultAsync(g => g.Id == gasto.IdGastoHH);
                    if (gastohhexistente != null)
                    {
                        gastohhexistente.Fechapago = gasto.Fechapago;
                        gastohhexistente.Hhtotales = gasto.HHtotales;
                        gastohhexistente.Monto = gasto.Monto;
                        gastohhexistente.Observacion = gasto.Observacion;
                        gastohhexistente.Subtotal = gasto.Subtotal;
                        gastohhexistente.Reajuste = gasto.Reajuste;
                    }
                    else
                    {
                        var nuevogastohh = new Gastoshhhejecucion
                        {
                            Idproyecto = idproyecto,
                            Tiporecurso = gasto.Tiporecurso,
                            Mes = gasto.Mes,
                            Anio = gasto.Anio,
                            Fechapago = gasto.Fechapago,
                            Monto = gasto.Monto,
                            Hhtotales = gasto.HHtotales,
                            Observacion = gasto.Observacion,
                            Subtotal = gasto.Subtotal,
                            Reajuste = gasto.Reajuste
                        };
                        await context.AddRangeAsync(nuevogastohh);
                    }
                   
                }
                await context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error al registrar/actualizar un gastos hh de ejecución: {e.Message}");
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
                                           Fecha = g.Fecha,
                                           Observacion = g.Observacion,
                                           Estado = g.Estado,
                                       }).ToListAsync();

                return resultado;
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error al obtener los gastos de ejecución: {e.Message}");
                return new List<GastosRealesDTO>();
            }
           
        }
        public async Task<List<GastosHHRecursosDTO>> ObtenerGastosHH(int? idproyecto)
        {
            try
            {


                var gastoshh = new List<GastosHHRecursosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERGASTOSHHEJECUCION";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GastosHHRecursosDTO datos = new()
                            {
                                idcuenta = reader.IsDBNull(reader.GetOrdinal("IDCUENTA")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                cuenta = reader.IsDBNull(reader.GetOrdinal("CUENTA")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTA")),
                                anio = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                tiporecurso = reader.IsDBNull(reader.GetOrdinal("TipoRecurso")) ? string.Empty : reader.GetString(reader.GetOrdinal("TipoRecurso")),
                                totalhh = reader.IsDBNull(reader.GetOrdinal("TotalHorasPorRecurso")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalHorasPorRecurso")),
                                costorecurso = reader.IsDBNull(reader.GetOrdinal("COSTORECURSO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTORECURSO")),
                                costoconsultoresexternos = reader.IsDBNull(reader.GetOrdinal("COSTOTOTALCONSULTORESEXTERNOS")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOTOTALCONSULTORESEXTERNOS")),
                            };
                            gastoshh.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return gastoshh;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el gastos hh  de los proyectos:{ex.Message}");
                return new List<GastosHHRecursosDTO>();

            }
        }
        public async Task<List<FlujoCajaDTO>> ObtenerFlujoCajaProyecto(int? idproyecto)
        {
            try
            {


                var flujo = new List<FlujoCajaDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTEFLUJOPROYECTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FlujoCajaDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                MontoProyectado = reader.IsDBNull(reader.GetOrdinal("Monto_Proyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Monto_Proyectado")),
                                MontoReal = reader.IsDBNull(reader.GetOrdinal("Monto_Real")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Monto_Real")),
                                Idcuenta = reader.IsDBNull(reader.GetOrdinal("Idcuenta")) ? 0 : reader.GetInt32(reader.GetOrdinal("Idcuenta")),
                                Cuenta = reader.IsDBNull(reader.GetOrdinal("Cuenta")) ? string.Empty : reader.GetString(reader.GetOrdinal("Cuenta")),
                                MontoPorCobrar = reader.IsDBNull(reader.GetOrdinal("MontoporCobrar")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoporCobrar")),
                            };
                            flujo.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return flujo;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el flujo  del proyecto:{ex.Message}");
                return new List<FlujoCajaDTO>();

            }
        }

        public async Task<List<EgresoFlujoCajaDTO>> ObtenerEgresoFlujoCajaProyecto(int? idproyecto)
        {
            try
            {


                var flujo = new List<EgresoFlujoCajaDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "REPORTEEGRESOFLUJOPROYECTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            EgresoFlujoCajaDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                MontosSocioProyectado = reader.IsDBNull(reader.GetOrdinal("MontoSocioProyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoSocioProyectado")),
                                MontoSocioReal = reader.IsDBNull(reader.GetOrdinal("MontoSocioReal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoSocioReal")),
                                MontosStaffProyectado = reader.IsDBNull(reader.GetOrdinal("MontoStaffProyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoStaffProyectado")),
                                MontoStaffReal = reader.IsDBNull(reader.GetOrdinal("MontoStaffReal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoStaffReal")),
                                MontosConsultoresProyectado = reader.IsDBNull(reader.GetOrdinal("MontoConsultoresProyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoConsultoresProyectado")),
                                MontoConsultoresReal = reader.IsDBNull(reader.GetOrdinal("MontoConsultoresReal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoConsultoresReal")),
                                
                            };
                            flujo.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return flujo;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el egreso flujo  del proyecto:{ex.Message}");
                return new List<EgresoFlujoCajaDTO>();

            }
        }

        public async Task<List<EgresosServiciosGastosDTO>> ObtenerEgresoServiciosFlujoCajaProyecto(int? idproyecto)
        {
            try
            {
            

                var flujo = new List<EgresosServiciosGastosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[REPORTEEGRESOFLUJOPROYECTOsERVICIOS]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            EgresosServiciosGastosDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                MontosServiciosProyectado = reader.IsDBNull(reader.GetOrdinal("MontoServiciosProyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoServiciosProyectado")),
                                MontoSerivicioReal = reader.IsDBNull(reader.GetOrdinal("MontoServiciosReal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoServiciosReal")),
                                ServicioNombre = reader.IsDBNull(reader.GetOrdinal("ServicioNombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("ServicioNombre")),
                                idcuentaservicio = reader.IsDBNull(reader.GetOrdinal("IdcuentaServicio")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdcuentaServicio")),
                                CuentaServicio = reader.IsDBNull(reader.GetOrdinal("CuentaServicio")) ? string.Empty : reader.GetString(reader.GetOrdinal("CuentaServicio")),
                                /* MontosGastosProyectado = reader.IsDBNull(reader.GetOrdinal("MontoGastosProyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoGastosProyectado")),
                                 MontoGastoReal = reader.IsDBNull(reader.GetOrdinal("MontoGastosReal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoGastosReal")),
                                 GastoNombre = reader.IsDBNull(reader.GetOrdinal("GastoNombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("GastoNombre")),
                                 idcuentaservicio = reader.IsDBNull(reader.GetOrdinal("IdcuentaServicio")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdcuentaServicio")),
                                 CuentaServicio = reader.IsDBNull(reader.GetOrdinal("CuentaServicio")) ? string.Empty : reader.GetString(reader.GetOrdinal("CuentaServicio")),
                                 idcuentagasto = reader.IsDBNull(reader.GetOrdinal("IdcuentaGasto")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdcuentaGasto")),
                                 Cuentagasto = reader.IsDBNull(reader.GetOrdinal("CuentaGasto")) ? string.Empty : reader.GetString(reader.GetOrdinal("CuentaGasto")),
                                */

                            };
                            flujo.Add(datos);
                            
                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return flujo;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el egreso flujo  del proyecto:{ex.Message}");
                return new List<EgresosServiciosGastosDTO>();

            }
        }

        public async Task<List<EgresosServiciosGastosDTO>> ObtenerEgresoGastosFlujoCajaProyecto(int? idproyecto)
        {
            try
            {


                var flujo = new List<EgresosServiciosGastosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[REPORTEEGRESOFLUJOPROYECTOGASTOS]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            EgresosServiciosGastosDTO datos = new()
                            {
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("Num_Proyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("Num_Proyecto")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                
                                MontosGastosProyectado = reader.IsDBNull(reader.GetOrdinal("MontoGastosProyectado")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoGastosProyectado")),
                                 MontoGastoReal = reader.IsDBNull(reader.GetOrdinal("MontoGastosReal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoGastosReal")),
                                 GastoNombre = reader.IsDBNull(reader.GetOrdinal("GastoNombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("GastoNombre")),
                               
                                 idcuentagasto = reader.IsDBNull(reader.GetOrdinal("IdcuentaGasto")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdcuentaGasto")),
                                 Cuentagasto = reader.IsDBNull(reader.GetOrdinal("CuentaGasto")) ? string.Empty : reader.GetString(reader.GetOrdinal("CuentaGasto")),
                                

                            };
                            flujo.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return flujo;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener el egreso flujo  del proyecto:{ex.Message}");
                return new List<EgresosServiciosGastosDTO>();

            }
        }
    }
}
