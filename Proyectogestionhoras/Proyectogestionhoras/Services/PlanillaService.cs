using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyectogestionhoras.Models;
using Proyectogestionhoras.Models.DTO;
using Proyectogestionhoras.Services.Interface;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
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
     
                var planilla = await context.Planillas.FirstOrDefaultAsync(p => p.IdUsuario == idusuario);
                if (planilla == null)
                {
                    planilla = new Planilla
                    {
                        IdUsuario = idusuario,
                    };
                    context.Planillas.Add(planilla);
                    await context.SaveChangesAsync();
                }

              
                bool existereigstro = await context.PlanillaUsusarioProyectos.AnyAsync(p => p.IdUsuProy == idusuproy && p.FechaRegistro.Date == Fecharegistro.Date);
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

                // Buscar el UsuarioProyecto
                var usuarioproyecto = await context.UsuarioProyectos
                .Include(up => up.IdUsuarioNavigation) 
                .ThenInclude(u => u.IdRecursoNavigation) 
                .FirstOrDefaultAsync(up => up.Id == idusuproy);

                if (usuarioproyecto != null)
                {
                    
                    if (usuarioproyecto.HhSocios.HasValue)
                    {
                        usuarioproyecto.HhSocios -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhStaff.HasValue)
                    {
                        usuarioproyecto.HhStaff -= horasasignadas;
                    }
                    else if (usuarioproyecto.HhConsultora.HasValue)
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
                                recurso.HhAnuales += horasasignadas; 
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

        public async Task<List<PlanillaUsuarioDTO>> ObtenerPlanillaUsuario(int idusuario)
        {
            try
            {

                var planillausuario = new List<PlanillaUsuarioDTO>();
                DbConnection connection = await conexion.OpenDatabaseConnectionAsync();
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "RECUPERARPLANILLAUSUARIO";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IDUSUARIO", idusuario));
                  
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PlanillaUsuarioDTO datos = new()
                            {
                                FechaRegistro =  reader.GetDateTime(reader.GetOrdinal("FECHA_REGISTRO")),
                                NombreProyecto = reader.IsDBNull(reader.GetOrdinal("NOMBREPROYECTO")) ? string.Empty : reader.GetString(reader.GetOrdinal("NOMBREPROYECTO")),
                                NumProyecto = reader.IsDBNull(reader.GetOrdinal("NUMPROYECTO")) ? null : reader.GetString(reader.GetOrdinal("NUMPROYECTO")),
                                IDPROYECTO = reader.IsDBNull(reader.GetOrdinal("IDPROYECTO")) ? 0 : reader.GetInt32(reader.GetOrdinal("IDPROYECTO")),
                                NombreActividad = reader.IsDBNull(reader.GetOrdinal("NOMBREACTIVIDAD")) ? null : reader.GetString(reader.GetOrdinal("NOMBREACTIVIDAD")),
                               
                                HHregistradas = reader.IsDBNull(reader.GetOrdinal("HHREGISTRADAS")) ? 0 : reader.GetInt32(reader.GetOrdinal("HHREGISTRADAS")),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("OBSERVACIONES")) ? null : reader.GetString(reader.GetOrdinal("OBSERVACIONES")),

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

    }
}
