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

                var idsIngresosEliminados = ingresos.Where(g => g.EsEliminado).Select(g => g.IdIngresoreal)
               .Where(id => id > 0)
               .ToList();

                if (idsIngresosEliminados.Any())
                {
                    var IngresosParaEliminar = await context.Ingresosreales
                                                          .Where(g => idsIngresosEliminados.Contains(g.Id))
                                                          .ToListAsync();
                    context.Ingresosreales.RemoveRange(IngresosParaEliminar);
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
                            ingresoExistente.Montous = 0;
                            ingresoExistente.Tc = 0;
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
                            Montous = 0,
                            Tc = 0,
                            Montoclp = ingreso.Montoclp,
                            Iva = ingreso.Iva,
                            Estado = ingreso.Estado,
                            Venta = "Vendido",
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

                var idsServiciosEliminados = servicios.Where(s => s.EsEliminado).Select(s => s.IdServicioReal)
                .Where(id => id > 0)
                .ToList();

                if (idsServiciosEliminados.Any())
                {
                    var serviciosParaEliminar = await context.Serviciosejecucions
                                                          .Where(s => idsServiciosEliminados.Contains(s.Id))
                                                          .ToListAsync();
                    context.Serviciosejecucions.RemoveRange(serviciosParaEliminar);
                }

                var idsServicios = servicios.Where(s => s.IdServicioReal > 0 && s.EsEliminado == false)
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
                        
                        servicioExistente.Tiposervicio = servicio.Tiposervicio;
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
                    Venta = "Vendido",
                    Tiposervicio = servicio.Tiposervicio,
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

                var idsGastosEliminados = gastos.Where(g => g.EsEliminado).Select(g => g.IdGastoReal)
                .Where(id => id > 0)
                .ToList();

                if (idsGastosEliminados.Any())
                {
                    var gastosParaEliminar = await context.Gastosejecucions
                                                          .Where(g => idsGastosEliminados.Contains(g.Id))
                                                          .ToListAsync();
                    context.Gastosejecucions.RemoveRange(gastosParaEliminar);
                }


                var idsGastos = gastos.Where(g => g.IdGastoReal > 0 && g.EsEliminado == false)
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
                    Venta = "Vendido",
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
                    return;



                var idsGastoshhEliminados = gastosHH.Where(g => g.EsEliminado).Select(g => g.IdGastoHH)
                .Where(id => id > 0)
                .ToList();

                if (idsGastoshhEliminados.Any())
                {
                    var gastosParaEliminar = await context.Gastoshhhejecucions
                                                          .Where(g => idsGastoshhEliminados.Contains(g.Id))
                                                          .ToListAsync();
                    context.Gastoshhhejecucions.RemoveRange(gastosParaEliminar);
                }



                var idsExistentes = gastosHH.Select(g => g.IdGastoHH).Where(id => id != null).ToList();
                var registrosExistentes = await context.Gastoshhhejecucions
                                                       .Where(g => idsExistentes.Contains(g.Id))
                                                       .ToListAsync();

                var nuevosRegistros = new List<Gastoshhhejecucion>();

                foreach (var gasto in gastosHH)
                {
                    if(gasto.Estado != 2)
                    {
                        gasto.Estado = gasto.Fechapago != null ? 1 : 0;
                    }
                    gasto.Reajuste ??= 0;
                    
                    gasto.Monto = gasto.Subtotal + gasto.Reajuste;

                    
                    var registroExistente = registrosExistentes.FirstOrDefault(g => g.Id == gasto.IdGastoHH);
                    if (registroExistente != null)
                    {
                        registroExistente.Fechapago = gasto.Fechapago;
                        registroExistente.Hhtotales = gasto.HHtotales;
                        registroExistente.Monto = gasto.Monto;
                        registroExistente.Observacion = gasto.Observacion;
                        registroExistente.Subtotal = gasto.Subtotal;
                        registroExistente.Reajuste = gasto.Reajuste;
                        registroExistente.Estado = gasto.Estado;
                        
                    }
                    else
                    {
                        
                        nuevosRegistros.Add(new Gastoshhhejecucion
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
                            Reajuste = gasto.Reajuste,
                            Estado = gasto.Estado,
                            
                        });
                    }
                }

                
                if (nuevosRegistros.Any())
                    await context.Gastoshhhejecucions.AddRangeAsync(nuevosRegistros);

                
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error al registrar/actualizar un gastos hh de ejecución: {e.Message}");
                if (e.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
            }
        }


        public async Task<DateTime> ObtenerFechaPagoMesActual()
        {
            int diaPago = await context.Diapagos.Select(d => d.Dia).FirstOrDefaultAsync();
            DateTime hoy = DateTime.Today;
            return new DateTime(hoy.Year, hoy.Month, diaPago);
        }
       

        public async Task PagoAutomaticoHH()
        {
            try
            {
                DateTime fechaPagoMesActual = await ObtenerFechaPagoMesActual();
                bool resultado = fechaPagoMesActual == DateTime.Today;

                if (resultado)
                {
                    var gastoshh = await context.Gastoshhhejecucions
                        .Where(g => g.Estado == 0 && g.IdproyectoNavigation.StatusProyecto == 2)
                        .ToListAsync();

                    if (gastoshh.Count > 0)
                    {
                        List<GastosHHViewModel> gastos = new List<GastosHHViewModel>();

                        foreach (var g in gastoshh)
                        {
                            gastos.Add(new GastosHHViewModel
                            {
                                IdGastoHH = g.Id,
                                Estado = 1,
                                Fechapago = fechaPagoMesActual,
                                HHtotales = g.Hhtotales,
                                Monto = g.Monto,
                                Observacion = g.Observacion,
                                Subtotal = g.Subtotal,
                                Reajuste = g.Reajuste,
                            });
                        }

                        // await GestorGastosHH(0, gastos);
                    }


                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al pagar las hh: {e.Message}");
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
                                           Venta = g.Venta,
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
                                idcuentasocio = reader.IsDBNull(reader.GetOrdinal("IDCUENTASOCIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTASOCIO")),
                                idcuentastaff = reader.IsDBNull(reader.GetOrdinal("IDCUENTASTAFF")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTASTAFF")),
                                cuentasocio = reader.IsDBNull(reader.GetOrdinal("CUENTASOCIO")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTASOCIO")),
                                cuentastaff = reader.IsDBNull(reader.GetOrdinal("CUENTASTAFF")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTASTAFF")),
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



        public async Task<List<GastosHHRecursosDTO>> ObtenerDistribucionHH(int? idproyecto, int? estado)
        {
            try
            {
                var resultado = await (
                    from g in context.Gastoshhhejecucions
                    join p in context.Proyectos on g.Idproyecto equals p.Id
                    join c in context.Historialcuentasproyectos on p.Id equals c.Idproyecto
                    join hc in context.HistorialCostosProyectos on p.Id equals hc.Idproyecto
                    where p.StatusProyecto == 2 &&
                          (estado == null || g.Estado == estado) &&
                          (idproyecto == null || p.Id == idproyecto)
                    group new { g, p, c, hc } by new
                    {
                        ProyectoId = p.Id,
                        p.Nombre,
                        g.Id,
                        g.Anio,
                        g.Mes,
                        g.Tiporecurso,
                        c.Idcuentasocio,
                        c.Idcuentastaff,
                        c.Cuentasocio,
                        c.Cuentastaff,
                        hc.Costosocio,
                        hc.Costostaff,
                        g.Reajuste,
                        g.Monto,
                        g.Observacion,
                        g.Fechapago,
                        g.Estado,
                    } into grp
                    select new GastosHHRecursosDTO
                    {
                        idproyecto = grp.Key.ProyectoId,
                        nombreproyecto = grp.Key.Nombre,
                        anio = grp.Key.Anio ?? 0,
                        mes = grp.Key.Mes ?? 0,
                        tiporecurso = grp.Key.Tiporecurso,
                        idcuentasocio = grp.Key.Idcuentasocio ?? 0,
                        idcuentastaff = grp.Key.Idcuentastaff ?? 0,
                        cuentasocio = grp.Key.Cuentasocio,
                        cuentastaff = grp.Key.Cuentastaff,
                        totalhh = grp.Sum(x => x.g.Hhtotales ?? 0),
                        costorecursosocio = grp.Sum(x => (x.g.Hhtotales ?? 0) * x.hc.Costosocio),
                        costorecurstaff = grp.Sum(x => (x.g.Hhtotales ?? 0) * x.hc.Costostaff),
                        costohhsocio = grp.Key.Costosocio,
                        costohhstaff = grp.Key.Costostaff,
                        idgastohh = grp.Key.Id,
                        reajuste = grp.Key.Reajuste ?? 0,
                        monto = grp.Key.Monto ?? 0,
                        observacion = grp.Key.Observacion,
                        fechapago = grp.Key.Fechapago ?? null,
                        estado = grp.Key.Estado ?? 0,
                    }
                ).OrderBy(g => g.anio).ThenBy(g => g.mes).ToListAsync();

                return resultado;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener el gastos hh  de los proyectos: {ex.Message}");
                return new List<GastosHHRecursosDTO>();
            }
        }


       


        public async Task<List<ProveedorForecastDTO>> ObtenerForecastProveedores(int? idproyecto)
        {
            try
            {


                var forecast = new List<ProveedorForecastDTO>();
#pragma warning disable CS8600
                object idproyectoparamater = (object)idproyecto ?? DBNull.Value;
               
#pragma warning restore CS8600
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[OBTENERFORECASTPROVEEDORES]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyectoparamater));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProveedorForecastDTO datos = new()
                            {
                                nombreproveedor = reader.IsDBNull(reader.GetOrdinal("nombreproveedor")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombreproveedor")),
                                tipoproveedor = reader.IsDBNull(reader.GetOrdinal("tipoproveedor")) ? string.Empty : reader.GetString(reader.GetOrdinal("tipoproveedor")),
                                cuenta = reader.IsDBNull(reader.GetOrdinal("cuenta")) ? string.Empty : reader.GetString(reader.GetOrdinal("cuenta")),
                                nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                                nombreproyecto = reader.IsDBNull(reader.GetOrdinal("nombreproyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombreproyecto")),
                                codigo = reader.IsDBNull(reader.GetOrdinal("codigo")) ? string.Empty : reader.GetString(reader.GetOrdinal("codigo")),

                                anio = reader.IsDBNull(reader.GetOrdinal("anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("anio")),
                                mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes")),
                                totalforecastproveedor = reader.IsDBNull(reader.GetOrdinal("totalforecastproveedor")) ? 0 : reader.GetDecimal(reader.GetOrdinal("totalforecastproveedor")),

                                
                            };
                            forecast.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return forecast;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los forecast de los proovedores:{ex.Message}");
                return new List<ProveedorForecastDTO>();

            }
        }

        public async Task<List<ProveedorForecastDTO>> ObtenerForecastIngresos()
        {
            try
            {


                var forecast = new List<ProveedorForecastDTO>();

                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[OBTENERFORECASTINGRESO]";
                    command.CommandType = CommandType.StoredProcedure;
            
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProveedorForecastDTO datos = new()
                            {
                                nombreproveedor = reader.IsDBNull(reader.GetOrdinal("nombreproveedor")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombreproveedor")),
                                
                                cuenta = reader.IsDBNull(reader.GetOrdinal("cuenta")) ? string.Empty : reader.GetString(reader.GetOrdinal("cuenta")),
                               
                                nombreproyecto = reader.IsDBNull(reader.GetOrdinal("nombreproyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombreproyecto")),
                                numproyecto = reader.IsDBNull(reader.GetOrdinal("numeroproyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("numeroproyecto")),
                                codigo = reader.IsDBNull(reader.GetOrdinal("codigo")) ? string.Empty : reader.GetString(reader.GetOrdinal("codigo")),

                                anio = reader.IsDBNull(reader.GetOrdinal("anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("anio")),
                                mes = reader.IsDBNull(reader.GetOrdinal("mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("mes")),
                                totalforecastproveedor = reader.IsDBNull(reader.GetOrdinal("totalforecastproveedor")) ? 0 : reader.GetDecimal(reader.GetOrdinal("totalforecastproveedor")),


                            };
                            forecast.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return forecast;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los forecast de los proovedores:{ex.Message}");
                return new List<ProveedorForecastDTO>();

            }
        }
    }
}
