using Proyectogestionhoras.Services.Interface;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Metadata;
using Proyectogestionhoras.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Collections.Generic;
namespace Proyectogestionhoras.Services
{
    public class ProyectoService : IProyecto
    {
        private readonly Conexion conexion;
        private readonly PROYECTO_CONTROL_HORASContext context;
        public ProyectoService(Conexion conexion,PROYECTO_CONTROL_HORASContext context) { 
            
            this.conexion = conexion;
            this.context = context;
        }



        public async Task<bool> CrearProyecto(decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, string numproyecto, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int idclientesucursal, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura, List<ServicioViewModel> servicios, List<GastoViewModel> gastos)
        {
            try
            {
                #pragma warning disable CS8600
                object probabilidadparameter = (object)probabilidad ?? DBNull.Value;
                object porcentajeparametr = (object)porcentajeprobabilidad ?? DBNull.Value;
                object fechaplazoparameter = (object)fechaplazoneg ?? DBNull.Value;
                #pragma warning restore CS8600
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CREAR_PROYECTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@MONTO", monto));
                    command.Parameters.Add(new SqlParameter("@MONEDA", moneda));
                    command.Parameters.Add(new SqlParameter("@AFECTAIVA", afectaiva));
                    command.Parameters.Add(new SqlParameter("@ID_TIPOLOGIA", idtipologia));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    command.Parameters.Add(new SqlParameter("@NUM_PROYECTO", numproyecto));
                    command.Parameters.Add(new SqlParameter("@FECHA_INICIO", fechainicio));
                    command.Parameters.Add(new SqlParameter("@FECHA_TERMINO ", fechatermino));
                    command.Parameters.Add(new SqlParameter("@PLAZO", plazo));
                    command.Parameters.Add(new SqlParameter("@TIPO_EMPRESA", tipoempresa));
                    command.Parameters.Add(new SqlParameter("@ID_CCOSTO_UNEGOCIO", codigoccosto));
                    command.Parameters.Add(new SqlParameter("@ID_CLIENTE_SUCURSAL", idclientesucursal));
                    command.Parameters.Add(new SqlParameter("@STATUS_PROYECTO", status));
                    command.Parameters.Add(new SqlParameter("@PROBABILIDAD", probabilidadparameter));
                    command.Parameters.Add(new SqlParameter("@PORCENTAJE_PROBABILIDAD", porcentajeparametr));
                    command.Parameters.Add(new SqlParameter("@FECHA_PLAZO_NEG", fechaplazoparameter));
                    command.Parameters.Add(new SqlParameter("@HHSOCIOS", hhsocios));
                   
                    command.Parameters.Add(new SqlParameter("@HHSTAFF", hhstaff));
                    
                    command.Parameters.Add(new SqlParameter("@HHCONSULTORA", hhconsultora));
                   
                    command.Parameters.Add(new SqlParameter("@HHCONSULTORB", hhconsultorb));
                 
                    command.Parameters.Add(new SqlParameter("@HHCONSULTORC", hhconsultorc));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOSOCIO", idsegmentosocio));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOSTAFF", idsegmentostaff));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCONSULTORA", idsegmentoconsultora));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCONSULTORB", idsegmentoconsultorb));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCONSULTORC", idsegmentoconsultorc));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOFACTURA", idsegmentofactura));
                    SqlParameter idProyectoParameter = new SqlParameter("@ID_PROYECTO", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idProyectoParameter);
                    
                    await command.ExecuteNonQueryAsync();
                    int idProyectoCreado = (int)idProyectoParameter.Value;
                    await GestorServiciosProyecto(idProyectoCreado, servicios);
                    await GestorProyectoGastos(idProyectoCreado, gastos);
                    return true;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al registrar el PROYECTO" + ex);
                return false;
            }
        }


      
        /*EDITAR PROYECTO*/
        public async Task<bool> EditarProyecto(int idproyecto, int idpresupuesto, decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura, List<ServicioViewModel> servicios, List<GastoViewModel> gastos, List<UsuarioProyectoViewModel> usuariohoras)
        {
            try
            {
#pragma warning disable CS8600
                object probabilidadparameter = (object)probabilidad ?? DBNull.Value;
                object porcentajeparametr = (object)porcentajeprobabilidad ?? DBNull.Value;
                object fechaplazoparameter = (object)fechaplazoneg ?? DBNull.Value;
#pragma warning restore CS8600
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "EDITAR_PROYECTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    command.Parameters.Add(new SqlParameter("@IDPRESUPUESTO", idpresupuesto));
                    command.Parameters.Add(new SqlParameter("@MONTO", monto));
                    command.Parameters.Add(new SqlParameter("@MONEDA", moneda));
                    command.Parameters.Add(new SqlParameter("@AFECTAIVA", afectaiva));
                    command.Parameters.Add(new SqlParameter("@ID_TIPOLOGIA", idtipologia));
                    command.Parameters.Add(new SqlParameter("@NOMBRE", nombre));
                    
                    command.Parameters.Add(new SqlParameter("@FECHA_INICIO", fechainicio));
                    command.Parameters.Add(new SqlParameter("@FECHA_TERMINO ", fechatermino));
                    command.Parameters.Add(new SqlParameter("@PLAZO", plazo));
                    command.Parameters.Add(new SqlParameter("@TIPO_EMPRESA", tipoempresa));
                    command.Parameters.Add(new SqlParameter("@ID_CCOSTO_UNEGOCIO", codigoccosto));
                    
                    command.Parameters.Add(new SqlParameter("@STATUS_PROYECTO", status));
                    command.Parameters.Add(new SqlParameter("@PROBABILIDAD", probabilidadparameter));
                    command.Parameters.Add(new SqlParameter("@PORCENTAJE_PROBABILIDAD", porcentajeparametr));
                    command.Parameters.Add(new SqlParameter("@FECHA_PLAZO_NEG", fechaplazoparameter));
                    command.Parameters.Add(new SqlParameter("@HHSOCIOS", hhsocios));

                    command.Parameters.Add(new SqlParameter("@HHSTAFF", hhstaff));

                    command.Parameters.Add(new SqlParameter("@HHCONSULTORA", hhconsultora));

                    command.Parameters.Add(new SqlParameter("@HHCONSULTORB", hhconsultorb));

                    command.Parameters.Add(new SqlParameter("@HHCONSULTORC", hhconsultorc));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOSOCIO", idsegmentosocio));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOSTAFF", idsegmentostaff));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCONSULTORA", idsegmentoconsultora));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCONSULTORB", idsegmentoconsultorb));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOCONSULTORC", idsegmentoconsultorc));
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTOFACTURA", idsegmentofactura));
                    
                    
                    await command.ExecuteNonQueryAsync();
                    await GestorServiciosProyecto(idproyecto, servicios);
                    await GestorProyectoGastos(idproyecto, gastos);
                    await AsignarHHUsuarios(idproyecto, usuariohoras);

                    return true;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al editar el PROYECTO" + ex);
                return false;
            }
        }

        public async Task GestorServiciosProyecto(int idProyecto, List<ServicioViewModel> servicios)
        {
     
            if (servicios == null || !servicios.Any())
            {
                return;
            }

            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();

            using (DbCommand command = connection.CreateCommand())
            {
           
                var serviciosExistentes = await context.ProyectoServicios
                    .Where(ps => ps.IdProyecto == idProyecto)
                    .ToListAsync();

  
                var serviciosAEliminar = serviciosExistentes
                    .Where(se => !servicios.Any(s => s.Idservicios == se.IdServicio))
                    .ToList();

         
                foreach (var servicioEliminado in serviciosAEliminar)
                {
                    context.ProyectoServicios.Remove(servicioEliminado);
                }

               
                foreach (var servicio in servicios)
                {
                    var servicioExistente = serviciosExistentes
                        .FirstOrDefault(se => se.IdServicio == servicio.Idservicios);

                    if (servicioExistente != null)
                    {
                       
                        servicioExistente.Idsegmento = servicio.IdSegmento;
                        servicioExistente.Monto = servicio.MontoServicio;
                    }
                    else
                    {
                      
                        var nuevoServicio = new ProyectoServicio
                        {
                            IdProyecto = idProyecto,
                            IdServicio = servicio.Idservicios,
                            Idsegmento = servicio.IdSegmento,
                            Monto = servicio.MontoServicio
                        };

                        await context.ProyectoServicios.AddAsync(nuevoServicio);
                    }
                }

                // Guardar los cambios
                await context.SaveChangesAsync();
            }
        }

        public async Task GestorProyectoGastos(int idProyecto, List<GastoViewModel> gastos)
        {
            if (gastos == null || !gastos.Any())
            {
                return;
            }

            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();


            var gastosExistentes = await context.ProyectoGastos
                .Where(pg => pg.IdProyecto == idProyecto)
                .ToListAsync();

       
            var idsGastosRecibidos = gastos.Select(g => g.Idgastos).ToList();


            foreach (var gasto in gastos)
            {
                var gastoExistente = gastosExistentes
                    .FirstOrDefault(g => g.IdGastos == gasto.Idgastos && g.Idsegmento == gasto.IdSegmento);

                if (gastoExistente != null)
                {
                
                    gastoExistente.Monto = gasto.MontoGasto;
                    context.ProyectoGastos.Update(gastoExistente);
                }
                else
                {
                    // Agregar nuevo gasto
                    var nuevoGasto = new ProyectoGasto
                    {
                        IdProyecto = idProyecto,
                        IdGastos = gasto.Idgastos,
                        Idsegmento = gasto.IdSegmento,
                        Monto = gasto.MontoGasto
                    };

                    await context.ProyectoGastos.AddAsync(nuevoGasto);
                }
            }

           
            var gastosAEliminar = gastosExistentes
                .Where(g => !idsGastosRecibidos.Contains(g.IdGastos))
                .ToList();

            if (gastosAEliminar.Any())
            {
                context.ProyectoGastos.RemoveRange(gastosAEliminar);
            }

        
            await context.SaveChangesAsync();
        }

        public async Task AsignarHHUsuarios(int idproyecto,List<UsuarioProyectoViewModel> usuariohoras)
        {
            if (usuariohoras == null || !usuariohoras.Any())
            {
                return;
            }
   
            var idsUsuarios = usuariohoras.Select(uh => uh.IdUsuario).ToList();

       
            var usuarios = await context.Usuarios
                .Include(u => u.IdRecursoNavigation)
                .Where(u => idsUsuarios.Contains(u.Id))
                .ToListAsync();

            var proyectosUsuarios = await context.UsuarioProyectos
                .Where(up => idsUsuarios.Contains(up.IdUsuario) && up.IdProyecto == idproyecto)
                .ToListAsync();

            foreach (var usuariovm in usuariohoras)
            {
                var usuario = usuarios.FirstOrDefault(u => u.Id == usuariovm.IdUsuario);
                if (usuario != null)
                {
                    var tiporecurso = usuario.IdRecursoNavigation.NombreRecurso;
                    var usuarioProyecto = proyectosUsuarios.FirstOrDefault(up => up.IdUsuario == usuariovm.IdUsuario);

                    if (usuarioProyecto != null)
                    {
                    
                        if (tiporecurso == "Socio")
                        {
                            usuarioProyecto.HhSocios = usuariovm.HHAsignadas;
                        }
                        else if (tiporecurso == "Staff")
                        {
                            usuarioProyecto.HhStaff = usuariovm.HHAsignadas;
                        }

                        var recurso = usuario.IdRecursoNavigation;
                        if (recurso.HhAnuales.HasValue)
                        {
                            recurso.HhAnuales -= usuariovm.HHAsignadas;
                        }
                    }
                }
            }

         
            await context.SaveChangesAsync();
        }


        public async Task<List<ProyectoDTO>> ObtenerProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto)
        {
            try
            {
                #pragma warning disable CS8600
                object idparameter = (object)id ?? DBNull.Value;
                object idclienteparameter = (object)idcliente ?? DBNull.Value;
                object nombreparameter = (object)nombre ?? DBNull.Value;
                object idtipoempresaparameter = (object)idtipoempresa ?? DBNull.Value;
                object statusproyectoparameter = (object)statusproyecto ?? DBNull.Value;
                object numproparameter = (object)numproyecto ?? DBNull.Value;
                object tipologiaparameter = (object)idtipologia ?? DBNull.Value;
                object unegocioparameter = (object)unidadneg ?? DBNull.Value;
                object ccostoparameter = (object)idccosto ?? DBNull.Value;
                #pragma warning restore CS8600
                var proyectos = new List<ProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using(DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "OBTENERPROYECTOS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ID",idparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDCLIENTE", idclienteparameter));
                    cmd.Parameters.Add(new SqlParameter("@NOMBRE", nombreparameter));
                    cmd.Parameters.Add(new SqlParameter("@ID_TIPOEMPRESA", idtipoempresaparameter));
                    cmd.Parameters.Add(new SqlParameter("@STATUS_PROYECTO", statusproyectoparameter));
                    cmd.Parameters.Add(new SqlParameter("@NUMPROYECTO", numproparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDTIPOLOGIA", tipologiaparameter));
                    cmd.Parameters.Add(new SqlParameter("@UNIDADNEGOCIO", unegocioparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDCENTROCOSTO", ccostoparameter));
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProyectoDTO proyecto = new() {

                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                numproyecto = reader.GetString(reader.GetOrdinal("NUM_PROYECTO")),
                                Tipo_Unegocio = reader.GetString(reader.GetOrdinal("TIPO_UNEGOCIO")),
                                IDUNEGOCIO = reader.GetInt32(reader.GetOrdinal("IDUNEGOCIO")),
                                IDCOSTO = reader.GetInt32(reader.GetOrdinal("IDCOSTO")),
                                Tipo_CCosto = reader.GetString(reader.GetOrdinal("TIPO_CCOSTO")),
                                Codigo = reader.GetString(reader.GetOrdinal("CODIGO")),
                                IDCLIENTE = reader.GetInt32(reader.GetOrdinal("IDCLIENTE")),
                                NombreCliente = reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                                NombreProyecto = reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                Tipo_Tipologia = reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                IDTIPOLOGIA = reader.GetInt32(reader.GetOrdinal("IDTIPOLOGIA")),
                                Tipo_Empresa = reader.GetString(reader.GetOrdinal("TIPO_EMPRESA")),
                                IDEMPRESA = reader.GetInt32(reader.GetOrdinal("IDEMPRESA")),
                                IDPRESUPESTO = reader.GetInt32(reader.GetOrdinal("IDPRESUPESTO")),
                                AfectaIva = reader.GetString(reader.GetOrdinal("AFECTAIVA")),
                                Tipo_Status = reader.GetString(reader.GetOrdinal("TIPO_STATUS")),
                                STATUSPROYECTO = reader.GetInt32(reader.GetOrdinal("STATUSPROYECTO")),
                                Probabilidad = reader.IsDBNull(reader.GetOrdinal("PROBABILIDAD")) ? string.Empty : reader.GetString(reader.GetOrdinal("PROBABILIDAD")),
                                Porcentaje_Probabilidad = reader.IsDBNull(reader.GetOrdinal("PORCENTAJE_PROBABILIDAD")) ? 0 : reader.GetDecimal(reader.GetOrdinal("PORCENTAJE_PROBABILIDAD")),
                                Plazo = reader.GetInt32(reader.GetOrdinal("PLAZO")),
                                Fecha_Inicio = reader.GetDateTime(reader.GetOrdinal("FECHA_INICIO")).Date,
                                Fecha_Termino = reader.GetDateTime(reader.GetOrdinal("FECHA_TERMINO")).Date,

                                Fecha_Plazo_Neg = reader.IsDBNull(reader.GetOrdinal("FECHA_PLAZO_NEG")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FECHA_PLAZO_NEG")),
                                NOMBREDEPARTAMENTO = reader.GetString(reader.GetOrdinal("NOMBREDEPARTAMENTO")),
                                IDDEPARTAMENTO = reader.GetInt32(reader.GetOrdinal("IDDEPARTAMENTO")),
                                MONTO = reader.IsDBNull(reader.GetOrdinal("MONTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                MONEDA = reader.IsDBNull(reader.GetOrdinal("MONEDA")) ? string.Empty : reader.GetString(reader.GetOrdinal("MONEDA")),

                                HHSOCIOS = reader.IsDBNull(reader.GetOrdinal("HH_SOCIOS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_SOCIOS")),
                                CUENTA_SOCIOS = reader.IsDBNull(reader.GetOrdinal("CUENTA_SOCIOS")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTA_SOCIOS")),
                                IDCUENTA_SOCIOS = reader.IsDBNull(reader.GetOrdinal("IDCUENTA_SOCIOS")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA_SOCIOS")),
                                SEGMENTO_SOCIOS = reader.IsDBNull(reader.GetOrdinal("SEGMENTO_SOCIOS")) ? string.Empty : reader.GetString(reader.GetOrdinal("SEGMENTO_SOCIOS")),
                                COSTO_SOCIO = reader.IsDBNull(reader.GetOrdinal("COSTO_SOCIO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_SOCIO")),

                                HHSTAFF = reader.IsDBNull(reader.GetOrdinal("HH_STAFF")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_STAFF")),
                                
                                CUENTA_STAFF = reader.IsDBNull(reader.GetOrdinal("CUENTA_STAFF")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTA_STAFF")),
                                IDCUENTA_STAFF = reader.IsDBNull(reader.GetOrdinal("IDCUENTA_STAFF")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA_STAFF")),
                                SEGMENTO_STAFF = reader.IsDBNull(reader.GetOrdinal("SEGMENTO_STAFF")) ? string.Empty : reader.GetString(reader.GetOrdinal("SEGMENTO_STAFF")),
                                COSTO_STAFF = reader.IsDBNull(reader.GetOrdinal("COSTO_STAFF")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_STAFF")),

                                HH_CONSULTOR_A = reader.IsDBNull(reader.GetOrdinal("HH_CONSULTOR_A")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_CONSULTOR_A")),
                                CUENTA_CONSULTOR_A = reader.IsDBNull(reader.GetOrdinal("CUENTA_CONSULTOR_A")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTA_CONSULTOR_A")),
                                IDCUENTA_CONSULTOR_A = reader.IsDBNull(reader.GetOrdinal("IDCUENTA_CONSULTOR_A")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA_CONSULTOR_A")),
                                SEGMENTO_CONSULTOR_A = reader.IsDBNull(reader.GetOrdinal("SEGMENTO_CONSULTOR_A")) ? string.Empty : reader.GetString(reader.GetOrdinal("SEGMENTO_CONSULTOR_A")),
                                COSTO_CONSULTORA = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORA")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORA")),

                                HH_CONSULTOR_B = reader.IsDBNull(reader.GetOrdinal("HH_CONSULTOR_B")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_CONSULTOR_B")),
                                CUENTA_CONSULTOR_B = reader.IsDBNull(reader.GetOrdinal("CUENTA_CONSULTOR_B")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTA_CONSULTOR_B")),
                                IDCUENTA_CONSULTOR_B = reader.IsDBNull(reader.GetOrdinal("IDCUENTA_CONSULTOR_B")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA_CONSULTOR_B")),
                                SEGMENTO_CONSULTOR_B = reader.IsDBNull(reader.GetOrdinal("SEGMENTO_CONSULTOR_B")) ? string.Empty : reader.GetString(reader.GetOrdinal("SEGMENTO_CONSULTOR_B")),
                                COSTO_CONSULTORB = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORB")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORB")),

                                HH_CONSULTOR_C = reader.IsDBNull(reader.GetOrdinal("HH_CONSULTOR_C")) ? 0 : reader.GetInt32(reader.GetOrdinal("HH_CONSULTOR_C")),
                                CUENTA_CONSULTOR_C = reader.IsDBNull(reader.GetOrdinal("CUENTA_CONSULTOR_C")) ? string.Empty : reader.GetString(reader.GetOrdinal("CUENTA_CONSULTOR_C")),
                                IDCUENTA_CONSULTOR_C = reader.IsDBNull(reader.GetOrdinal("IDCUENTA_CONSULTOR_C")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA_CONSULTOR_C")),
                                SEGMENTO_CONSULTOR_C = reader.IsDBNull(reader.GetOrdinal("SEGMENTO_CONSULTOR_C")) ? string.Empty : reader.GetString(reader.GetOrdinal("SEGMENTO_CONSULTOR_C")),
                                COSTO_CONSULTORC = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORC")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORC"))

                                



                            };
                          
                            proyectos.Add(proyecto);
                            
                        }
                    }
                    await conexion.CloseDatabaseConnectionAsync();
                    return proyectos;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los proyectos" + ex);
                return new List<ProyectoDTO>();

            }
        }

        public async Task<List<ServiciosProyectoDTO>> ObtenerServiciosProyecto(int? idproyecto)
        {
            try
            {
                var serviciosProyectos = new List<ServiciosProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSERVICIOSPROYECTO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ServiciosProyectoDTO servicio = new()
                            {
                                IDSERVICIO = reader.GetInt32(reader.GetOrdinal("IDSERVICIO")),
                                NOMBRESERVICIO = reader.GetString(reader.GetOrdinal("NOMBRESERVICIO")),
                                NOMBRSEGMENTO = reader.GetString(reader.GetOrdinal("NOMBRSEGMENTO")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                MONTO = reader.GetDecimal(reader.GetOrdinal("MONTO")),
                            };
                            serviciosProyectos.Add(servicio);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return serviciosProyectos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los servicios del proyecto:" + ex.Message);
                return new List<ServiciosProyectoDTO>();
            }
        }

        public async Task<List<GastoProyectoDTO>> ObtenerGastosProyectos(int? idproyecto)
        {
            try
            {
                var gastosproyectos = new List<GastoProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERGASTOSPROYECTO ";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GastoProyectoDTO gasto = new()
                            {
                                IDGASTOS = reader.GetInt32(reader.GetOrdinal("IDGASTOS")),
                                NOMBREGASTO = reader.GetString(reader.GetOrdinal("NOMBREGASTO")),
                                NOMBRSEGMENTO = reader.GetString(reader.GetOrdinal("NOMBRSEGMENTO")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                MONTO = reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))
                            };
                            gastosproyectos.Add(gasto);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return gastosproyectos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los sesrvicios del proyecto:" + ex.Message);
                return new List<GastoProyectoDTO>();
            }
        }


        public async Task<List<Unegocio>> ObtenerUnegocio()
        {
            try
            {
                var unegocios = new List<Unegocio>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERUNEGOCIO";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Unegocio unegocio = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoUnegocio = reader.GetString(reader.GetOrdinal("TIPO_UNEGOCIO")),

                            };
                            unegocios.Add(unegocio);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return unegocios;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los unegocios:" + ex.Message);
                return new List<Unegocio>();
            }
        }
        public async Task<List<Ccosto>> ObtenerCcosto()
        {
            try
            {
                var ccostos = new List<Ccosto>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCCOSTO";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Ccosto ccosto = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoCcosto = reader.GetString(reader.GetOrdinal("TIPO_CCOSTO")),

                            };
                            ccostos.Add(ccosto);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return ccostos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los ccostos:" + ex.Message);
                return new List<Ccosto>();
            }
        }

        public async Task<List<CcostoUnegocio>> ObtenerCodigoCCosto(int idcosto, int idunegocio)
        {
            try
            {
                var ccostonegocios = new List<CcostoUnegocio>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCODIGOCCOSTO ";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO ", idunegocio));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            CcostoUnegocio ccostounegocio = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Codigo = reader.GetString(reader.GetOrdinal("CODIGO")),

                            };
                            ccostonegocios.Add(ccostounegocio);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return ccostonegocios;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los unegocios:" + ex.Message);
                return new List<CcostoUnegocio>();
            }
        }
        public async Task<List<Empresa>> ObtenerEmpresa()
        {
            try
            {
                var empresas = new List<Empresa>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERTIPOEMPRESA";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Empresa empresa = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoEmpresa = reader.GetString(reader.GetOrdinal("TIPO_EMPRESA")),

                            };
                            empresas.Add(empresa);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return empresas;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las empresas:" + ex.Message);
                return new List<Empresa>();
            }
        }
        public async Task<List<Tipologium>> ObtenerTipoligias()
        {
            try
            {
                var tipologias = new List<Tipologium>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERTIPOLOGIA";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Tipologium tipologia = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoTipologia = reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),

                            };
                            tipologias.Add(tipologia);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return tipologias;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<Tipologium>();
            }
        }

        public async Task<List<StatusProyecto>> ObtenerStatus()
        {
            try
            {
                var status = new List<StatusProyecto>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSTATUSFICHA";
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            StatusProyecto statu = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                TipoStatus = reader.GetString(reader.GetOrdinal("TIPO_STATUS")),

                            };
                            status.Add(statu);
                            Debug.WriteLine($"TIPOSTATUS:"+statu.TipoStatus);
                        }
                        
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return status;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<StatusProyecto>();
            }
        }
        public async Task<List<FacturaDTO>> ObtenerValoresFactura(int idcosto, int idunegocio)
        {
            try
            {
                var facturatablas = new List<FacturaDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GENERARTABLASEGMENTOFACTURA";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO ", idunegocio));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FacturaDTO factura = new()
                            {
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))
                            };
                            facturatablas.Add(factura);
                           
                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return facturatablas;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<FacturaDTO>();
            }
        }
        public async Task<List<ServiciosDTO>> ObtenerValoresServicios(int idcosto, int idunegocio)
        {
            try
            {
                var servicios = new List<ServiciosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSEGMENTOTERCEROS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO", idunegocio));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ServiciosDTO servicio = new()
                            {
                                NOMBRE = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))
                            };
                            servicios.Add(servicio);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return servicios;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<ServiciosDTO>();
            }
        }
        public async Task<List<Segmento>> GetValoresSegmentos(int idcodigo)
        {
            try
            {
                var segmentos = new List<Segmento>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSEGMENTOGASTOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCODIGOCOSTO", idcodigo));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Segmento segmento = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                

                            };
                            segmentos.Add(segmento);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return segmentos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<Segmento>();
            }
        }
      /*  public async Task<List<Cuentum>> GetValoresGastos(int idsegmento)
        {
            try
            {
                var cuentas = new List<Cuentum>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERCUENTAGASTOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDSEGMENTO", idsegmento));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Cuentum cuentum = new()
                            {
                                Idcuenta = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                Cuenta = reader.GetString(reader.GetOrdinal("CUENTA")),


                            };
                            cuentas.Add(cuentum);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return cuentas;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<Cuentum>();
            }
        }*/
        public async Task<List<Gasto>> ObtenerGastos()
        {
            try
            {
                var gastos = new List<Gasto>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERGASTOS";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Gasto gasto = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),


                            };
                            gastos.Add(gasto);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return gastos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<Gasto>();
            }
        }

        public async Task<List<Servicio>> ObtenerServicios()
        {
            try
            {
                var servicios = new List<Servicio>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSERVICIOS";
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Servicio servicio = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Nombre = reader.GetString(reader.GetOrdinal("NOMBRE")),


                            };
                            servicios.Add(servicio);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return servicios;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<Servicio>();
            }
        }
        public async Task<List<ConsultoresDTO>> ObtenerValoresConsultores(int idcosto, int idunegocio)
        {
            try
            {
                var consultores = new List<ConsultoresDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSEGMENTOSCONSULTORES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO", idunegocio));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ConsultoresDTO consultor = new()
                            {
                                NOMBRE = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))
                            };
                            consultores.Add(consultor);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return consultores;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<ConsultoresDTO>();
            }
        }
        public async Task<List<HonorariosDTO>> ObtenerValoresHonorarios(int idcosto, int idunegocio, int idrecurso)
        {
            try
            {
                var honorarios = new List<HonorariosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "OBTENERSEGMENTOHONORARIOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO", idunegocio));
                    command.Parameters.Add(new SqlParameter("@IDRECURSO", idrecurso));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HonorariosDTO honorario = new()
                            {
                                NOMBRE = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))

                            };
                            honorarios.Add(honorario);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return honorarios;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las tipologias:" + ex.Message);
                return new List<HonorariosDTO>();
            }
        }

        public async Task<List<GastoDTO>> ObtenerValoresGastos(int idcodigo, string nombregasto)
        {
            try
            {
                var gastos = new List<GastoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "FILTRARSEGMENTOGASTOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCOSDIGOCOSTO", idcodigo));
                    command.Parameters.Add(new SqlParameter("@NOMBRESEGMENTO", nombregasto));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GastoDTO gasto = new()
                            {
                                NOMBRE = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))

                            };
                            gastos.Add(gasto);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return gastos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los gastos:" + ex.Message);
                return new List<GastoDTO>();
            }
        }

        public async Task<List<GastoDTO>> ObtenerValoresGastosEdicion(int idcosto,int unegocio, string nombresegmento)
        {
            try
            {
                var gastos = new List<GastoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "FILTRARSEGMENTOGASTOSEDICION";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO", unegocio));
                    command.Parameters.Add(new SqlParameter("@NOMBRESEGMENTO", nombresegmento));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GastoDTO gasto = new()
                            {
                                NOMBRE = reader.GetString(reader.GetOrdinal("NOMBRE")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO"))

                            };
                            gastos.Add(gasto);

                        }

                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return gastos;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los gastos:" + ex.Message);
                return new List<GastoDTO>();
            }
        }
    }
}
