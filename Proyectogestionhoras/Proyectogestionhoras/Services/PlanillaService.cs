using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Proyectogestionhoras.Services
{
    public class PlanillaService : IPlanilla
    {
        private readonly PROYECTO_CONTROL_HORASContext context;
        private readonly Conexion conexion;
        public PlanillaService(PROYECTO_CONTROL_HORASContext context, Conexion conexion)
        {
            this.context = context;
            this.conexion = conexion;
        }


        public async Task<int> RegistrarHoras(int idusuario, int idusuproy, int horasasignadas, DateTime Fecharegistro, string? observaciones, int idactividad)
        {
            try
            {
                int mesregistro = Fecharegistro.Month;
                int anioregistro = Fecharegistro.Year;

                var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == idusuario && p.Mes == mesregistro && p.Anio == anioregistro);
                if (planilla == null)
                {
                    planilla = new Planilla
                    {
                        IdUsuario = idusuario,
                        Mes = mesregistro,
                        Anio = anioregistro,
                    };
                    context.Planillas.Add(planilla);
                    await context.SaveChangesAsync();
                }


                bool existereigstro = await context.PlanillaUsusarioProyectos.AnyAsync(p => p.IdUsuProy == idusuproy && p.FechaRegistro.Date == Fecharegistro.Date && p.IdActividad == idactividad);
                if (existereigstro)
                {
                    return 2;
                }

                var registro = new PlanillaUsusarioProyecto
                {
                    IdPlanilla = planilla.Id,
                    IdUsuProy = idusuproy,
                    RegistroHhProyecto = horasasignadas,
                    FechaRegistro = Fecharegistro,
                    Observaciones = observaciones,
                    IdActividad = idactividad,
                };
                context.PlanillaUsusarioProyectos.Add(registro);

                var inicioSemana = Fecharegistro.AddDays(-(int)Fecharegistro.DayOfWeek + (int)DayOfWeek.Monday);
                var finSemana = inicioSemana.AddDays(6);
                var horasRegistradasSemana = await context.PlanillaUsusarioProyectos
                    .Join(
                    context.UsuarioProyectos,
                    planilla => planilla.IdUsuProy,
                    usuarioProyecto => usuarioProyecto.Id,
                    (planilla, usuarioProyecto) => new { planilla, usuarioProyecto }
                    )
                    .Where(joinResult => joinResult.usuarioProyecto.IdUsuario == idusuario
                         && joinResult.planilla.FechaRegistro >= inicioSemana
                         && joinResult.planilla.FechaRegistro <= finSemana)
                    .SumAsync(joinResult => joinResult.planilla.RegistroHhProyecto);


                var usuarioproyecto = await context.UsuarioProyectos
                .Include(up => up.IdUsuarioNavigation)
                .ThenInclude(u => u.IdRecursoNavigation)
                .FirstOrDefaultAsync(up => up.Id == idusuproy);

                if (usuarioproyecto != null)
                {

                    /*if (usuarioproyecto.HhSocios.HasValue)
                    {
                        usuarioproyecto.HhSocios -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhStaff.HasValue)
                    {
                        usuarioproyecto.HhStaff -= horasasignadas;
                    }*/
                    if (usuarioproyecto.HhConsultora.HasValue)
                    {
                        usuarioproyecto.HhConsultora -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhConsultorb.HasValue)
                    {
                        usuarioproyecto.HhConsultorb -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhConsultorc.HasValue)
                    {
                        usuarioproyecto.HhConsultorc -= horasasignadas;
                    }


                    var usuario = usuarioproyecto.IdUsuarioNavigation;


                    if (usuario != null && usuario.IdRecurso != 0)
                    {
                        var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                        if (recurso != null)
                        {

                            if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                            {
                                recurso.HhAnuales -= horasasignadas;
                                if (recurso.NombreRecurso == "Socio")
                                {
                                   

                                    var usuariosRelacionados = await context.UsuarioProyectos
                                    .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                    .ToListAsync();

                                    // Resta las horas asignadas a los socios relacionados
                                    foreach (var usuarioRelacionado in usuariosRelacionados)
                                    {
                                        usuarioRelacionado.HhSocios -= horasasignadas;
                                    }
                                }
                                // Solo para Staff
                                else if (recurso.NombreRecurso == "Staff")
                                {
                                   

                                    var usuariosRelacionados = await context.UsuarioProyectos
                                    .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                    .ToListAsync();

                                    // Resta las horas asignadas al staff relacionado
                                    foreach (var usuarioRelacionado in usuariosRelacionados)
                                    {
                                        usuarioRelacionado.HhStaff -= horasasignadas;
                                    }
                                }


                                decimal? totalpermitidossemana = recurso.NumeroHoras * (recurso.ProcentajeProyecto / 100);
                                Debug.WriteLine(totalpermitidossemana);
                                if (horasRegistradasSemana + horasasignadas > totalpermitidossemana)
                                {
                                    Debug.WriteLine("Error: Se exceden las horas permitidas en la semana.");
                                    return 3;
                                }
                                



                            }
                        }
                        else
                        {
                            Debug.WriteLine("Error al obtener el recurso asociado al usuario.");
                            return 2;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("El usuario no es válido o no tiene un recurso asociado.");
                        return 2;
                    }

                    await context.SaveChangesAsync();
                    return 1;
                }
                else
                {
                    Debug.WriteLine("Error: UsuarioProyecto no se encontró.");
                    return 2;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al registrar la hora en la planilla: {ex}");
                return 2;
            }
        }

        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaUsuario(int? idusuario, int? idplanilla)
        {
            try
            {
#pragma warning disable CS8600
                object idusuarioparameter = (object)idusuario ?? DBNull.Value;
                object idplanillaparameter = (object)idplanilla ?? DBNull.Value;
               
#pragma warning restore CS8600
                var planillausuario = new List<PlanillaUsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "RECUPERARPLANILLAUSUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuarioparameter));
                    command.Parameters.Add(new SqlParameter("@IDPLANILLA", idplanillaparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IDPLANILLA")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPLANILLA")),
                                FechaRegistro =  reader.GetDateTime(reader.GetOrdinal("FECHA_REGISTRO")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("NUMPROYECTO")) ? null : reader.GetString(reader.GetOrdinal("NUMPROYECTO")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("IDPROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPROYECTO")),
                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NOMBREACTIVIDAD")) ? null : reader.GetString(reader.GetOrdinal("NOMBREACTIVIDAD")),
                               
                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHREGISTRADAS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HHREGISTRADAS")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("OBSERVACIONES")) ? null : reader.GetString(reader.GetOrdinal("OBSERVACIONES")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("MES")) ? 0 : reader.GetInt32(reader.GetOrdinal("MES")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("ANIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ANIO")),
                            };
                            planillausuario.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return planillausuario;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener la planilla del usuario:" + ex.Message);
                return new List<PlanillaUsuarioDTO>();

            }
        }
        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaExcel(int idplanilla)
        {
            try
            {

                var planillausuario = new List<PlanillaUsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "RECUPERARPLANILLAUSUARIO_EXCEL";
                    command.CommandType = CommandType.StoredProcedure;
                 
                    command.Parameters.Add(new SqlParameter("@IDPLANILLA", idplanilla));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IDPLANILLA")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPLANILLA")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FECHA_REGISTRO")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("NUMPROYECTO")) ? null : reader.GetString(reader.GetOrdinal("NUMPROYECTO")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("IDPROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPROYECTO")),
                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NOMBREACTIVIDAD")) ? null : reader.GetString(reader.GetOrdinal("NOMBREACTIVIDAD")),

                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHREGISTRADAS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HHREGISTRADAS")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("OBSERVACIONES")) ? null : reader.GetString(reader.GetOrdinal("OBSERVACIONES")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("MES")) ? 0 : reader.GetInt32(reader.GetOrdinal("MES")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("ANIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ANIO")),
                                IDCUENTA = reader.IsDBNull(reader.GetOrdinal("IDCUENTA")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDCUENTA")),
                                Cuenta = reader.IsDBNull(reader.GetOrdinal("CUENTA")) ? null : reader.GetString(reader.GetOrdinal("CUENTA")),
                                NombreSegmento = reader.IsDBNull(reader.GetOrdinal("NOMBRESEGMENTO")) ? null : reader.GetString(reader.GetOrdinal("NOMBRESEGMENTO")),
                            };
                            planillausuario.Add(datos);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return planillausuario;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener la planilla del usuario:" + ex.Message);
                return new List<PlanillaUsuarioDTO>();

            }
        }
   
        public async Task<List<ProyectoGantt>> ObtenerDatosGantt(int idusuario)
        {
            var proyectosGantt = new List<ProyectoGantt>();
            var datosGantt = new List<DatosGanttDTO>();

            try
            {
                using (DbConnection connection = await conexion.OpenDatabaseConnectionAsync())
                {
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_GetHorasPorUsuarioPorMes";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@IdUsuario", idusuario));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var datos = new DatosGanttDTO
                                {
                                    NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NombreProyecto")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreProyecto")),
                                    Año = reader.IsDBNull(reader.GetOrdinal("Año")) ? 0 : reader.GetInt32(reader.GetOrdinal("Año")),
                                    Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                    TotalHoras = reader.IsDBNull(reader.GetOrdinal("TotalHoras")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalHoras")),
                                    FechaInicio = reader.IsDBNull(reader.GetOrdinal("Fecha_Inicio")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Fecha_Inicio")),
                                    FechaTermino = reader.IsDBNull(reader.GetOrdinal("Fecha_Termino")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Fecha_Termino")),
                                };

                            
                                datosGantt.Add(datos);
                            }
                        }
                    }
                }

               
                proyectosGantt = datosGantt
                    .GroupBy(d => d.NombreProyecto)
                    .Select(g => new ProyectoGantt
                    {
                        NombreProyecto = g.Key,
                        FechaInicio = g.First().FechaInicio, 
                        FechaTermino = g.First().FechaTermino,
                        HorasPorMes = g.GroupBy(d => new { d.Año, d.Mes })
                                       .Select(m => new HorasPorMes
                                       {
                                           Año = m.Key.Año,
                                           Mes = m.Key.Mes,
                                           Horas = m.Sum(x => x.TotalHoras) // Sumar las horas totales por año y mes
                                       })
                                       .ToList() 
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hubo un error al obtener los datos de la gantt: {ex.Message}");
                // Manejar la excepción según sea necesario
            }

            return proyectosGantt;
        }

        public async Task<List<HHUSUARIOPROYECTOTOTALDTO>> RecuperarHHUsuarios(int idusuario)
        {
            try
            {

                var proyectoshh = new List<HHUSUARIOPROYECTOTOTALDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "HH_USUARIO_PROYECTOS_GANTT";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            HHUSUARIOPROYECTOTOTALDTO hhproyecto = new()
                            {
                                IDUSUARIOPROYCTO = reader.IsDBNull(reader.GetOrdinal("IDUSUARIOPROYCTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDUSUARIOPROYCTO")),
                                IDUSUARIO = reader.IsDBNull(reader.GetOrdinal("ID_USUARIO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_USUARIO")),
                                Recurso = reader.IsDBNull(reader.GetOrdinal("RECURSO")) ? null : reader.GetString(reader.GetOrdinal("RECURSO")),
                                Nombre_Usuario = reader.IsDBNull(reader.GetOrdinal("NOMBRE_USUARIO")) ? null : reader.GetString(reader.GetOrdinal("NOMBRE_USUARIO")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("ID_PROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID_PROYECTO")),
                                HHRESTANTES = reader.IsDBNull(reader.GetOrdinal("HHASIGNADAS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HHASIGNADAS")),
                                NOMBREPROYECTO = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? null : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),

                            };
                            proyectoshh.Add(hhproyecto);

                        }
                    }

                }
                await conexion.CloseDatabaseConnectionAsync();
                return proyectoshh;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Hubo un error al obtener los clientes:" + ex.Message);
                return new List<HHUSUARIOPROYECTOTOTALDTO>();

            }
        }



    }
}
