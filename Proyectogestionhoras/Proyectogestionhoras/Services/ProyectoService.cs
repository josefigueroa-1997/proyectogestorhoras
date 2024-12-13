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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;
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



        public async Task<bool> CrearProyecto(decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, string numproyecto, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int idclientesucursal, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura,  decimal montoorigen,List<ServicioViewModel> servicios, List<GastoViewModel> gastos)
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
                    command.Parameters.Add(new SqlParameter("@MONTOMONEDAORIGEN", montoorigen));
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
        public async Task<bool> EditarProyecto(int idproyecto, int idpresupuesto, decimal monto, string moneda, string afectaiva, int idtipologia, string nombre, DateTime fechainicio, DateTime fechatermino, int plazo, int tipoempresa, int codigoccosto, int status, string? probabilidad, decimal? porcentajeprobabilidad, DateTime? fechaplazoneg, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc, int idsegmentofactura, decimal montoorigen, List<ServicioViewModel> servicios, List<GastoViewModel> gastos)
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
                    command.Parameters.Add(new SqlParameter("@MONTOMONEDAORIGEN", montoorigen));
                    
                    
                    await command.ExecuteNonQueryAsync();
                    await GestorServiciosProyecto(idproyecto, servicios);
                    await GestorProyectoGastos(idproyecto, gastos);
                  /* var resultadoAsignacion = await AsignarHHUsuarios(idproyecto, usuariohoras);
                    if (resultadoAsignacion == 2)
                    {
                        // Si no hay suficientes horas, lanzamos una excepción o manejamos el error de otra forma
                        throw new Exception("No hay suficientes horas anuales para asignar a uno o más usuarios.");
                    }*/

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

            var serviciosExistentes = await context.ProyectoServicios
                .Where(ps => ps.IdProyecto == idProyecto)
                .ToListAsync();

            
            foreach (var servicio in servicios.Where(s => s.EsEliminado))
            {
                var servicioExistente = serviciosExistentes
                    .FirstOrDefault(se => se.Id == servicio.IdServicioProyecto);

                if (servicioExistente != null)
                {
                    context.ProyectoServicios.Remove(servicioExistente);
                }
            }

            
            foreach (var servicio in servicios.Where(s => !s.EsEliminado))
            {
                if (servicio.IdServicioProyecto > 0)
                {
                    var servicioExistente = serviciosExistentes
                   .FirstOrDefault(se => se.Id == servicio.IdServicioProyecto);

                    if (servicioExistente != null)
                    {
                        servicioExistente.Idsegmento = servicio.IdSegmento;
                        servicioExistente.Monto = servicio.MontoServicio;
                        servicioExistente.IdServicio = servicio.Idservicios;

                    }
                }
               
                else
                {
                    var nuevoServicio = new ProyectoServicio
                    {
                        IdProyecto = idProyecto,
                        IdServicio = servicio.Idservicios,
                        Idsegmento = servicio.IdSegmento,
                        Monto = servicio.MontoServicio,
                        Fecha = servicio.Fecha.Date,
                        
                        
                    };

                    await context.ProyectoServicios.AddAsync(nuevoServicio);
                }
            }

            await context.SaveChangesAsync();
        }


        public async Task GestorProyectoGastos(int idProyecto, List<GastoViewModel> gastos)
        {
            if (gastos == null || !gastos.Any())
            {
                return;
            }

            var gastoExistentes = await context.ProyectoGastos
                .Where(ps => ps.IdProyecto == idProyecto)
                .ToListAsync();

            
            foreach (var gasto in gastos.Where(s => s.EsEliminado))
            {
                var gastoExistente = gastoExistentes
                    .FirstOrDefault(se => se.Id == gasto.IdGastoProyecto);

                if (gastoExistente != null)
                {
                    context.ProyectoGastos.Remove(gastoExistente);
                }
            }

            
            foreach (var gasto in gastos.Where(s => !s.EsEliminado))
            {
                if (gasto.IdGastoProyecto > 0)
                {
                    var gastoExistente = gastoExistentes
                   .FirstOrDefault(se => se.Id == gasto.IdGastoProyecto);

                    if (gastoExistente != null)
                    {
                        gastoExistente.Idsegmento = gasto.IdSegmento;
                        gastoExistente.Monto = gasto.MontoGasto;
                        gastoExistente.IdGastos = gasto.Idgastos;

                    }
                }
               
                else
                {
                    var nuevoGasto = new ProyectoGasto
                    {
                        IdProyecto = idProyecto,
                        IdGastos = gasto.Idgastos,
                        Idsegmento = gasto.IdSegmento,
                        Monto = gasto.MontoGasto,
                        Fecha = gasto.Fecha.Date,
                        
                    };

                    await context.ProyectoGastos.AddAsync(nuevoGasto);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task RestarHHAnaulesSocios(int hhsocios, int? idproyecto)
        {
            var socio = context.TotalRecursos
                       .Where(tr => tr.TipoRecurso == "Socio" && tr.Anio == DateTime.Now.Year)
                       .FirstOrDefault();

            if (socio != null)
            {
              
                var hhsociosasignadas = context.HhUsuarioHistorials
                                              .Where(u => u.IdProyecto == idproyecto)
                                              .FirstOrDefault();

                decimal horasAsignadasPrevias = hhsociosasignadas?.HhSocios ?? 0;

                
                decimal diferenciaHoras = hhsocios - horasAsignadasPrevias;

                Debug.WriteLine($"Horas asignadas previas: {horasAsignadasPrevias}, Nuevas horas: {hhsocios}, Diferencia: {diferenciaHoras}");

                socio.TotalHhAnuales -= diferenciaHoras;

                await context.SaveChangesAsync();
            }
        }

        public async Task RestarHHAnaulesStaff(int hhstaff, int? idproyecto)
        {
            
            var staff = context.TotalRecursos
                               .Where(tr => tr.TipoRecurso == "Staff" && tr.Anio == DateTime.Now.Year)
                               .FirstOrDefault();

            if (staff != null)
            {
                
                var hhstaffasignadas = context.HhUsuarioHistorials
                                              .Where(u => u.IdProyecto == idproyecto)
                                              .FirstOrDefault();

                decimal horasAsignadasPrevias = hhstaffasignadas?.HhStaff ?? 0;

                decimal diferenciaHoras = hhstaff - horasAsignadasPrevias;

                staff.TotalHhAnuales -= diferenciaHoras;

                await context.SaveChangesAsync();
            }
            else
            {
                Debug.WriteLine("No se encontró el recurso Staff para el año actual.");
            }
        }


        public async Task ReasignarHHRecursos(int idproyecto, int hhsocios, int hhstaff, int hhconsultora, int hhconsultorb, int hhconsultorc, int idsegmentosocio, int idsegmentostaff, int idsegmentoconsultora, int idsegmentoconsultorb, int idsegmentoconsultorc)
        {

            DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "REASIGNACIONHHRECURSOS";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyecto));

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
                


                await command.ExecuteNonQueryAsync();
                
            }
        }

        public async Task<int> AsignarHHUsuarios(int idproyecto, List<UsuarioProyectoViewModel> usuariohoras)
        {
            if (usuariohoras == null || !usuariohoras.Any())
            {
                return 0; 
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
               
                        var recurso = usuario.IdRecursoNavigation;
                        if (recurso.HhAnuales.HasValue && recurso.HhAnuales.Value < usuariovm.HHAsignadas)
                        {
                            return 2; 
                        }

                        
                        if (tiporecurso == "Socio")
                        {
                            usuarioProyecto.HhSocios = usuariovm.HHAsignadas;
                        }
                        else if (tiporecurso == "Staff")
                        {
                            usuarioProyecto.HhStaff = usuariovm.HHAsignadas;
                        }

                    
                        if (recurso.HhAnuales.HasValue)
                        {
                            recurso.HhAnuales -= usuariovm.HHAsignadas;
                        }

                 
                        if (usuariovm.HHAsignadas == 0)
                        {
                            context.UsuarioProyectos.Remove(usuarioProyecto);
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
            return 1; 
        }


        public async Task<List<ProyectoDTO>> ObtenerProyectos(int? id, int? idcliente, string? nombre, int? idtipoempresa, int? statusproyecto, string? numproyecto, int? idtipologia, int? unidadneg, int? idccosto, int? idusuario)
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
                object idusuarioparameter = (object)idusuario ?? DBNull.Value;
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
                    cmd.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuarioparameter));
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
                                COSTO_CONSULTORC = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORC")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORC")),
                                /*PRESUPUESTO
                                 */
                                MontoPresupuesto = reader.IsDBNull(reader.GetOrdinal("MONTOPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTOPRESUPUESTO")),
                                CostoSocioPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOSOCIOPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOSOCIOPRESUPUESTO")),
                                CostoStaffPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOSTAFFPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOSTAFFPRESUPUESTO")),
                                CostoConsultorAPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORAPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORAPRESUPUESTO")),
                                CostoConsultorBPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORBPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORBPRESUPUESTO")),
                                CostoConsultorCPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORCPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORCPRESUPUESTO")),
                                MontoOrigenExtranjero = reader.IsDBNull(reader.GetOrdinal("MONTOMONEDAORIGEN")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTOMONEDAORIGEN")),
                                idpresupuesto = reader.IsDBNull(reader.GetOrdinal("IDPRESUPUESTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPRESUPUESTO")),

                                



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
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                IDSERVICIO = reader.GetInt32(reader.GetOrdinal("IDSERVICIO")),
                                NOMBRESERVICIO = reader.GetString(reader.GetOrdinal("NOMBRESERVICIO")),
                                NOMBRSEGMENTO = reader.GetString(reader.GetOrdinal("NOMBRSEGMENTO")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                MONTO = reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                FECHA = reader.GetDateTime(reader.GetOrdinal("FECHA")),
                                
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
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                IDGASTOS = reader.GetInt32(reader.GetOrdinal("IDGASTOS")),
                                NOMBREGASTO = reader.GetString(reader.GetOrdinal("NOMBREGASTO")),
                                NOMBRSEGMENTO = reader.GetString(reader.GetOrdinal("NOMBRSEGMENTO")),
                                CUENTA = reader.GetString(reader.GetOrdinal("CUENTA")),
                                IDCUENTA = reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                MONTO = reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                IDSEGMENTO = reader.GetInt32(reader.GetOrdinal("IDSEGMENTO")),
                                FECHA = reader.GetDateTime(reader.GetOrdinal("FECHA")),
                                
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


        public async Task<List<HistorailNegociacionDTO>> RecuperarHistorialNegociacion(int? idproyecto,int? idnegociacion)
        {
            try
            {
#pragma warning disable CS8600
                object idproyectoparameter = (object)idproyecto ?? DBNull.Value;
                object idnegociacionparameter = (object)idnegociacion ?? DBNull.Value;
#pragma warning restore CS8600
                var negociaciones = new List<HistorailNegociacionDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "TRAER_NEGOCIACIONES";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyectoparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDNEGOCIACION", idnegociacionparameter));
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HistorailNegociacionDTO historial = new()
                            {
                                IdNegociacion = reader.IsDBNull(reader.GetOrdinal("IDNEGOCIACION")) ? default : reader.GetInt32(reader.GetOrdinal("IDNEGOCIACION")),
                                Id = reader.IsDBNull(reader.GetOrdinal("ID")) ? default : reader.GetInt32(reader.GetOrdinal("ID")),
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("NUM_PROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NUM_PROYECTO")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBRE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRE")),
                                AfectaIva = reader.IsDBNull(reader.GetOrdinal("AFECTAIVA")) ? string.Empty : reader.GetString(reader.GetOrdinal("AFECTAIVA")),
                                Monto = reader.IsDBNull(reader.GetOrdinal("MONTO")) ? default : reader.GetDecimal(reader.GetOrdinal("MONTO")),
                                Plazo = reader.IsDBNull(reader.GetOrdinal("PLAZO")) ? default : reader.GetInt32(reader.GetOrdinal("PLAZO")),
                                Fechainicio = reader.IsDBNull(reader.GetOrdinal("FECHAINICIO")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("FECHAINICIO")),
                                FechaTermino = reader.IsDBNull(reader.GetOrdinal("FECHATERMINO")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("FECHATERMINO")),
                                FechaPlazoNeg = reader.IsDBNull(reader.GetOrdinal("FECHANEGOCIACION")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("FECHANEGOCIACION")),
                                Probabilidad = reader.IsDBNull(reader.GetOrdinal("PROBABILIDAD")) ? string.Empty : reader.GetString(reader.GetOrdinal("PROBABILIDAD")),
                                NivelProbabilidad = reader.IsDBNull(reader.GetOrdinal("NIVELPROBABILIDAD")) ? default : reader.GetDecimal(reader.GetOrdinal("NIVELPROBABILIDAD")),
                                Hhsocios = reader.IsDBNull(reader.GetOrdinal("HHSOCIOS")) ? default : reader.GetInt32(reader.GetOrdinal("HHSOCIOS")),
                                Hhstaff = reader.IsDBNull(reader.GetOrdinal("HHSTAFF")) ? default : reader.GetInt32(reader.GetOrdinal("HHSTAFF")),
                                Hhconsultora = reader.IsDBNull(reader.GetOrdinal("HHCONSULTORA")) ? default : reader.GetInt32(reader.GetOrdinal("HHCONSULTORA")),
                                hhconsultorb = reader.IsDBNull(reader.GetOrdinal("HHCONSULTORB")) ? default : reader.GetInt32(reader.GetOrdinal("HHCONSULTORB")),
                                hhconsultorc = reader.IsDBNull(reader.GetOrdinal("HHCONSULTORC")) ? default : reader.GetInt32(reader.GetOrdinal("HHCONSULTORC")),
                                Fecha = reader.IsDBNull(reader.GetOrdinal("FECHA")) ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("FECHA")),
                                Tipologia = reader.IsDBNull(reader.GetOrdinal("TIPO_TIPOLOGIA")) ? string.Empty : reader.GetString(reader.GetOrdinal("TIPO_TIPOLOGIA")),
                                CentroCosto = reader.IsDBNull(reader.GetOrdinal("CODIGO")) ? string.Empty : reader.GetString(reader.GetOrdinal("CODIGO")),
                                NombreCliente = reader.IsDBNull(reader.GetOrdinal("NOMBRECLIENTE")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBRECLIENTE")),
                                NombreDepartamento = reader.IsDBNull(reader.GetOrdinal("NOMBREDEPARTAMENTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREDEPARTAMENTO")),
                                COSTOSOCIO = reader.IsDBNull(reader.GetOrdinal("COSTOSOCIO")) ? default : reader.GetDecimal(reader.GetOrdinal("COSTOSOCIO")),
                                CostoStaff = reader.IsDBNull(reader.GetOrdinal("COSTOSTAFF")) ? default : reader.GetDecimal(reader.GetOrdinal("COSTOSTAFF")),
                                CostoConsA = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORA")) ? default : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORA")),
                                CostoConsB = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORB")) ? default : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORB")),
                                CostoConsC = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORC")) ? default : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORC")),


                            };

                            negociaciones.Add(historial);

                        }
                    }
                    await conexion.CloseDatabaseConnectionAsync();
                    return negociaciones;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener las negociaciones" + ex);
                return new List<HistorailNegociacionDTO>();

            }
        }

        public async Task<List<FacturaNegociacionDTO>> RecuperarFacturaNegociacion(int? idnegociacion)
        {
            try
            {
                var factura = new List<FacturaNegociacionDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GENERARFACTURAPROYECTONEGOCIACION";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDNEGOCIO", idnegociacion));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            FacturaNegociacionDTO datos = new()
                            {
                                FechaFactura = reader.GetDateTime(reader.GetOrdinal("Fecha_Factura")),
                                Neto = reader.GetDecimal(reader.GetOrdinal("Neto")),
                                Iva = reader.GetDecimal(reader.GetOrdinal("IVA")),
                                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                
                            };
                            factura.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return factura;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener las facturas de la negociación:" + ex.Message);
                return new List<FacturaNegociacionDTO>();
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

        public async Task<List<GastoDTO>> ObtenerValoresGastosEdicion(int idcosto,int unegocio, int idgasto)
        {
            try
            {
                var gastos = new List<GastoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "FILTRARCUENTAGASTOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO", unegocio));
                    command.Parameters.Add(new SqlParameter("@IDGASTO", idgasto));
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

        public async Task<List<ServiciosDTO>> ObtenerValoresServiciosEdicion(int idcosto, int unegocio, int idservicio)
        {
            try
            {
                var servicios = new List<ServiciosDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "FILTRARCUENTASERVICIOS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDCOSTO", idcosto));
                    command.Parameters.Add(new SqlParameter("@IDUNEGOCIO", unegocio));
                    command.Parameters.Add(new SqlParameter("@IDSERVICIO", idservicio));
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
                Debug.WriteLine($"Hubo un error al obtener la cuenta de los servicios:" + ex.Message);
                return new List<ServiciosDTO>();
            }
        }


        public async Task<List<ProyectoDTO>> ObtenerPresupuestoProyectos(int? idpresupuesto,int? idproyecto, int? idcliente)
        {
            try
            {
#pragma warning disable CS8600
                object idpresupuestoparameter = (object)idpresupuesto ?? DBNull.Value;
                object idproyectoapramterer = (object)idproyecto ?? DBNull.Value;
                object idclientepramemter = (object)idcliente ?? DBNull.Value;
                
#pragma warning restore CS8600
                var proyectos = new List<ProyectoDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "OBTENERPROYECTOPRESUPUESTO";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@IDPRESUPUESTO", idpresupuestoparameter));
                    cmd.Parameters.Add(new SqlParameter("@IDPROYECTO", idproyectoapramterer));
                    cmd.Parameters.Add(new SqlParameter("@IDCLIENTE", idclientepramemter));
                    
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProyectoDTO proyecto = new()
                            {

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
                                COSTO_CONSULTORC = reader.IsDBNull(reader.GetOrdinal("COSTO_CONSULTORC")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTO_CONSULTORC")),
                                /*PRESUPUESTO*/
                                MontoPresupuesto = reader.IsDBNull(reader.GetOrdinal("MONTOPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTOPRESUPUESTO")),
                                CostoSocioPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOSOCIOPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOSOCIOPRESUPUESTO")),
                                CostoStaffPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOSTAFFPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOSTAFFPRESUPUESTO")),
                                CostoConsultorAPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORAPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORAPRESUPUESTO")),
                                CostoConsultorBPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORBPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORBPRESUPUESTO")),
                                CostoConsultorCPresupuesto = reader.IsDBNull(reader.GetOrdinal("COSTOCONSULTORCPRESUPUESTO")) ? 0 : reader.GetDecimal(reader.GetOrdinal("COSTOCONSULTORCPRESUPUESTO")),
                               MontoOrigenExtranjero = reader.IsDBNull(reader.GetOrdinal("MONTOMONEDAORIGEN")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MONTOMONEDAORIGEN")),
                                idpresupuesto = reader.IsDBNull(reader.GetOrdinal("IDPRESUPUESTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPRESUPUESTO")),





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
    }
}
