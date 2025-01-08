using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
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

        public async Task<int> RegistrarHorasPlanills(int idusuario, int? idusuproy, string horasasignadas, DateTime Fecharegistro, string? observaciones, int Idactividad, int? idsubactividad)
        {
            try
            {
                int resultado = 0;
                if (idusuproy.HasValue && idusuproy > 0 && (!idsubactividad.HasValue || idsubactividad == 0))
                {
                    resultado = await RegistrarHoras(idusuario, idusuproy.Value, horasasignadas, Fecharegistro, observaciones, Idactividad);
                }
                else if (idsubactividad.HasValue && idsubactividad > 0 && (!idusuproy.HasValue || idusuproy == 0))
                {
                    resultado = await RegistrarHorasEmpresa(idusuario, horasasignadas, Fecharegistro, observaciones, idsubactividad.Value);
                }
                return resultado;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Hubo un error al registrar horas en la planilla:{e.Message}");
                return 0;
            }
        }



        public async Task<int> RegistrarHoras(int idusuario, int idusuproy, string horasasignadas, DateTime Fecharegistro, string? observaciones, int Idactividad)
        {
            try
            {
                int statusProyecto = await context.UsuarioProyectos.Include(up => up.IdProyectoNavigation) .Where(up => up.Id == idusuproy) .Select(up => up.IdProyectoNavigation.StatusProyecto) .FirstOrDefaultAsync();
                string tipo = string.Empty;
                
                if(statusProyecto == 2)
                {
                    tipo = "En Ejecución";
                }
                else if(statusProyecto == 1)
                {
                    tipo = "En Negociación";
                }
                else
                {
                    return 2;
                }
                bool existereigstro = await context.PlanillaUsusarioProyectos.AnyAsync(p => p.IdUsuProy == idusuproy && p.FechaRegistro.Date == Fecharegistro.Date && p.Idactividad == Idactividad);
                if (existereigstro)
                {
                    return 2;
                }
                /* var proyecto = ObtenerProyectoPorUsuarioProyectoId(idusuproy);
                 DateTime fechainicio = proyecto.FechaInicio;
                 DateTime fechatermino = proyecto.FechaTermino;
                 if (Fecharegistro < fechainicio || Fecharegistro > fechatermino)
                 {
                     return 4;
                 }*/
                decimal horasAsignadasDecimal;
                if (!decimal.TryParse(horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                {

                    return 0;
                }
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

                var registro = new PlanillaUsusarioProyecto
                {
                    IdPlanilla = planilla.Id,
                    IdUsuProy = idusuproy,
                    RegistroHhProyecto = horasAsignadasDecimal,
                    FechaRegistro = Fecharegistro,
                    Observaciones = observaciones,
                    Idactividad = Idactividad,
                    Tipo = tipo,
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
                if (statusProyecto == 1)
                {
                    await context.SaveChangesAsync();
                    return 1;
                }
                if (usuarioproyecto != null)
                {


                    if (usuarioproyecto.HhConsultora.HasValue)
                    {
                        usuarioproyecto.HhConsultora -= horasAsignadasDecimal;
                    }
                    else if (usuarioproyecto.HhConsultorb.HasValue)
                    {
                        usuarioproyecto.HhConsultorb -= horasAsignadasDecimal;
                    }
                    else if (usuarioproyecto.HhConsultorc.HasValue)
                    {
                        usuarioproyecto.HhConsultorc -= horasAsignadasDecimal;
                    }


                    var usuario = usuarioproyecto.IdUsuarioNavigation;

                    if (statusProyecto == 2)
                    {
                        if (usuario != null && usuario.IdRecurso != 0)
                        {
                            var recurso = await context.Recursos.FindAsync(usuario.IdRecurso);
                            if (recurso != null)
                            {

                                if (recurso.NombreRecurso == "Socio" || recurso.NombreRecurso == "Staff")
                                {

                                    if (recurso.NombreRecurso == "Socio")
                                    {


                                        var usuariosRelacionados = await context.UsuarioProyectos
                                        .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhSocios.HasValue)
                                        .ToListAsync();


                                        foreach (var usuarioRelacionado in usuariosRelacionados)
                                        {
                                            usuarioRelacionado.HhSocios -= horasAsignadasDecimal;
                                        }
                                    }

                                    else if (recurso.NombreRecurso == "Staff")
                                    {


                                        var usuariosRelacionados = await context.UsuarioProyectos
                                        .Where(up => up.IdProyecto == usuarioproyecto.IdProyecto && up.HhStaff.HasValue)
                                        .ToListAsync();


                                        foreach (var usuarioRelacionado in usuariosRelacionados)
                                        {
                                            usuarioRelacionado.HhStaff -= horasAsignadasDecimal;
                                        }
                                    }


                                    /* decimal? totalpermitidossemana = recurso.NumeroHoras * (recurso.ProcentajeProyecto / 100);
                                     Debug.WriteLine(totalpermitidossemana);
                                     if (horasRegistradasSemana + horasAsignadasDecimal > totalpermitidossemana)
                                     {
                                         Debug.WriteLine("Error: Se exceden las horas permitidas en la semana.");
                                         return 3;
                                     }*/




                                }
                            }
                        }
                        
                        else
                        {
                            Debug.WriteLine("Error al obtener el recurso asociado al usuario.");
                            return 2;
                        }
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
        public Proyecto ObtenerProyectoPorUsuarioProyectoId(int idUsuarioProyecto)
        {


            var usuarioProyecto = context.UsuarioProyectos
                .Include(up => up.IdProyectoNavigation)
                .FirstOrDefault(up => up.Id == idUsuarioProyecto);


            return usuarioProyecto?.IdProyectoNavigation;

        }


        public async Task<int> RegistrarHorasEmpresa(int idusuario, string horasasignadas, DateTime Fecharegistro, string? observaciones, int idsubactividad)
        {

            try
            {

                bool existereigstro = await context.PlanillaRegistroEmpresas.AnyAsync(p => p.Idsubactividad == idsubactividad && p.Fecharegistro == Fecharegistro.Date && p.IdPlanillaNavigation.IdUsuarioNavigation.Id == idusuario);
                if (existereigstro)
                {
                    return 2;
                }

                decimal horasAsignadasDecimal;
                if (!decimal.TryParse(horasasignadas, NumberStyles.Any, CultureInfo.InvariantCulture, out horasAsignadasDecimal))
                {

                    return 0;
                }
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

                var registro = new PlanillaRegistroEmpresa
                {
                    IdPlanilla = planilla.Id,

                    Hhregistradas = horasAsignadasDecimal,
                    Fecharegistro = Fecharegistro,
                    Observaciones = observaciones,
                    Idsubactividad = idsubactividad,
                };
                context.PlanillaRegistroEmpresas.Add(registro);
                await context.SaveChangesAsync();
                return 1;

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
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IdPlanilla")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdPlanilla")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),


                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NombreActividad")) ? null : reader.GetString(reader.GetOrdinal("NombreActividad")),

                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHregistradas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHregistradas")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
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


        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaEmpresaUsuario(int? idusuario, int? idplanilla)
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
                    command.CommandText = "recuperarplanillaempresa";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuarioparameter));
                    command.Parameters.Add(new SqlParameter("@IDPLANILLA", idplanillaparameter));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IdPlanilla")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdPlanilla")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),


                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NombreActividad")) ? null : reader.GetString(reader.GetOrdinal("NombreActividad")),

                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHregistradas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHregistradas")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
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
                                IdPlanilla = reader.IsDBNull(reader.GetOrdinal("IdPlanilla")) ? 0 : reader.GetInt32(reader.GetOrdinal("IdPlanilla")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NombreActividad")) ? null : reader.GetString(reader.GetOrdinal("NombreActividad")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHregistradas")) ? 0 : reader.GetDecimal(reader.GetOrdinal("HHregistradas")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                                Mes = reader.IsDBNull(reader.GetOrdinal("Mes")) ? 0 : reader.GetInt32(reader.GetOrdinal("Mes")),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio")) ? 0 : reader.GetInt32(reader.GetOrdinal("Anio")),
                                NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreUsuario")),
                                Rol = reader.IsDBNull(reader.GetOrdinal("Rol")) ? string.Empty : reader.GetString(reader.GetOrdinal("Rol")),
                                CostoUnitario = reader.IsDBNull(reader.GetOrdinal("CostoUnitario")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CostoUnitario")),
                                CostoTotal = reader.IsDBNull(reader.GetOrdinal("CostoTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("CostoTotal")),
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

                Debug.WriteLine($"Hubo un error al obtener la planilla excel del usuario:" + ex.Message);
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

                Debug.WriteLine($"Hubo un error al obtener los gantt:" + ex.Message);
                return new List<HHUSUARIOPROYECTOTOTALDTO>();

            }
        }



    }
}
